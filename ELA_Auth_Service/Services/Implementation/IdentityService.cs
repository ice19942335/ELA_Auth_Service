using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using ELA_Auth_Service.Contracts.V1.Requests.Authentication.Auth;
using ELA_Auth_Service.Contracts.V1.Requests.Authentication.Email;
using ELA_Auth_Service.Contracts.V1.Requests.Authentication.Password;
using ELA_Auth_Service.Data;
using ELA_Auth_Service.Data._MySqlDataContext;
using ELA_Auth_Service.Domain.DTO;
using ELA_Auth_Service.Domain.Entities;
using ELA_Auth_Service.IdentityInitializer;
using ELA_Auth_Service.Options;
using ELA_Auth_Service.Services.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;

namespace ELA_Auth_Service.Services.Implementation
{
    public class IdentityService : IIdentityService
    {
        private readonly IEmailService _emailService;
        private readonly IConfiguration _configuration;
        private readonly UserManager<AppUser> _userManager;
        private readonly JwtSettings _jwtSettings;
        private readonly TokenValidationParameters _tokenValidationParameters;
        private readonly DataContext _context;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly string _clientUrl;
        private readonly MySqlDataContext _mySqlDataContext;
        private readonly ILogger<IdentityService> _logger;

        public IdentityService(
            IEmailService emailService,
            IConfiguration configuration,
            UserManager<AppUser> userManager,
            JwtSettings jwtSettings,
            TokenValidationParameters tokenValidationParameters,
            DataContext context,
            RoleManager<IdentityRole> roleManager,
            MySqlDataContext mySqlDataContext,
            ILogger<IdentityService> logger)
        {
            _emailService = emailService;
            _configuration = configuration;
            _userManager = userManager;
            _jwtSettings = jwtSettings;
            _tokenValidationParameters = tokenValidationParameters;
            _context = context;
            _roleManager = roleManager;
            _clientUrl = configuration["ClientSidePasswordResetPath"];
            _mySqlDataContext = mySqlDataContext;
            _logger = logger;
        }

        public async Task<PasswordUpdateDto> PasswordResetRequestAsync(string email)
        {
            var user = await _userManager.FindByEmailAsync(email);

            if (user is null)
                return new PasswordUpdateDto { Errors = new[] { "DataServiceUser with this email does not exist" }, CriticalError = true };
            else if (!await _userManager.IsEmailConfirmedAsync(user))
                return new PasswordUpdateDto { Errors = new[] { "User email is not confirmed, sorry, we can't help you with this" } };

            var secretCode = await _userManager.GeneratePasswordResetTokenAsync(user);

            var callBackUrl = $"<a href='{_clientUrl}?userId={user.Id}&code={secretCode}'>Click this link to reset password</a>";

            var sendEmailResult = await _emailService.SendEmailAsync(user.Email, "ELA Password reset", callBackUrl);

            if (!sendEmailResult.Success)
                return new PasswordUpdateDto { Errors = new[] { "Something went wrong, please try again or contact support" } };

            return new PasswordUpdateDto { Success = true };
        }

        public async Task<PasswordUpdateDto> PasswordUpdateAsync(PasswordUpdateRequest request)
        {
            var user = await _userManager.FindByIdAsync(request.UserId);

            if (user is null)
                return new PasswordUpdateDto { Errors = new[] { "This user does not exist anymore" }, CriticalError = true };

            var passwordUpdateResult = await _userManager.ResetPasswordAsync(user, request.Code, request.Password);

            if (!passwordUpdateResult.Succeeded)
            {
                if (passwordUpdateResult.Errors.FirstOrDefault(x => x.Description == "Invalid token.") is null)
                    return new PasswordUpdateDto { Errors = passwordUpdateResult.Errors.Select(x => x.Description) };

                return new PasswordUpdateDto
                {
                    Errors = passwordUpdateResult.Errors.Select(x => x.Description),
                    CriticalError = true
                };
            }

            var refreshToken = _context.RefreshTokens.FirstOrDefault(x => x.UserId == user.Id);

            if (refreshToken is null)
                return new PasswordUpdateDto
                {
                    Errors = new[] { "Can't find refresh token for this user, this is miracle!" },
                    CriticalError = true
                };

            refreshToken.Invalidated = true;
            await _context.SaveChangesAsync();

            return new PasswordUpdateDto { Success = true };
        }

        public async Task<EmailConfirmationDto> SendEmailConfirmationRequestAsync(EmailConfirmationRequest request)
        {
            var user = await _userManager.FindByEmailAsync(request.Email);

            if (user is null)
                return new EmailConfirmationDto { Errors = new[] { "User does not exist anymore" } };

            var secretCode = await _userManager.GenerateEmailConfirmationTokenAsync(user);

            var callBackUrl = $"<a href='{_clientUrl}?userId={user.Id}&code={secretCode}'>Click this link to confirm e-mail</a>";

            var sendEmailConfirmationLinkResult = await _emailService.SendEmailAsync(user.Email, "ELA E-mail confirmation", callBackUrl);

            if (!sendEmailConfirmationLinkResult.Success)
                return new EmailConfirmationDto { Errors = new[] { "Something went wrong, please try again or contact support" }, CriticalError = true };

            return new EmailConfirmationDto { Success = true };
        }

        public async Task<EmailConfirmationDto> ConfirmEmailAsync(ConfirmEmailRequest request)
        {
            var user = await _userManager.FindByIdAsync(request.UserId);

            if (user is null)
                return new EmailConfirmationDto
                {
                    Errors = new[] { "This user does not exist anymore" },
                    CriticalError = true
                };

            var emailConfirmationResult = await _userManager.ConfirmEmailAsync(user, request.Code);

            if (!emailConfirmationResult.Succeeded)
            {
                return new EmailConfirmationDto
                {
                    Errors = new[] { "Something went wrong, please try again or contact support" },
                    CriticalError = true
                };
            }

            return new EmailConfirmationDto { Success = true };
        }

        public async Task<AuthenticationDto> RegisterAsync(UserRegistrationRequest request)
        {
            var existingUser = await _userManager.FindByEmailAsync(request.Email);

            if (existingUser != null)
                return new AuthenticationDto { Errors = new[] { "User with this email address already exists" } };

            var newUserGuid = Guid.NewGuid();
            var newUser = new AppUser
            {
                Id = newUserGuid.ToString(),
                Email = request.Email,
                UserName = request.Email,
                Name = request.Name
            };

            var created = await _userManager.CreateAsync(newUser, request.Password);
            if (!created.Succeeded)
                return new AuthenticationDto { Errors = created.Errors.Select(x => x.Description) };

            await _userManager.AddToRoleAsync(newUser, DefaultIdentity.RoleUser);

            var addUserInMySqlDb = await _mySqlDataContext.CreateUser(newUserGuid, request.Name, 0);

            if (!addUserInMySqlDb)
                return new AuthenticationDto { Errors = new[] { "Problem on writing entry in MySqlDB" }, CriticalError = true };

            await SendEmailConfirmationRequestAsync(new EmailConfirmationRequest { Email = request.Email });

            return await GenerateAuthenticationResultForUserAsync(newUser);
        }

        public async Task<AuthenticationDto> LoginAsync(string email, string password)
        {
            var user = await _userManager.FindByEmailAsync(email);

            if (user is null)
            {
                _logger.LogWarning($"[AUTH FAILED] Email: {email}");
                return new AuthenticationDto { Errors = new[] { "User does not exist" } };
            }

            var userHasValidPassword = await _userManager.CheckPasswordAsync(user, password);

            if (!userHasValidPassword)
            {
                _logger.LogWarning($"[AUTH FAILED] Email: {email}");
                return new AuthenticationDto { Errors = new[] { "Password is incorrect" } };
            }

            _logger.LogInformation($"[User: {user.Email} Id: {user.Id}] Successfully logged in");
            return await GenerateAuthenticationResultForUserAsync(user);
        }

        public async Task<AuthenticationDto> RefreshTokenAsync(string token, string refreshToken)
        {
            var validatedToken = GetPrincipalFromToken(token);

            if (validatedToken is null)
                return new AuthenticationDto { Errors = new[] { "Invalid Token" }, CriticalError = true };

            var expiryDateUnix =
                long.Parse(validatedToken.Claims.Single(x => x.Type == JwtRegisteredClaimNames.Exp).Value);

            var expiryDateTimeUtc = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc)
                .AddSeconds(expiryDateUnix);

            if (expiryDateTimeUtc > DateTime.UtcNow)
                return new AuthenticationDto { Errors = new[] { "This token hasn't expired jet" } };

            var jti = validatedToken.Claims.Single(x => x.Type == JwtRegisteredClaimNames.Jti).Value;

            var storedRefreshToken = await _context.RefreshTokens.SingleOrDefaultAsync(x => x.Token == refreshToken);

            if (storedRefreshToken is null)
                return new AuthenticationDto { Errors = new[] { "This refresh token does not exist" }, CriticalError = true };

            if (DateTime.UtcNow > storedRefreshToken.ExpiryDate)
                return new AuthenticationDto { Errors = new[] { "This refresh token has expired" }, CriticalError = true };

            if (storedRefreshToken.Invalidated)
                return new AuthenticationDto { Errors = new[] { "This refresh token has been invalidated" }, CriticalError = true };

            if (storedRefreshToken.Used)
                return new AuthenticationDto { Errors = new[] { "This refresh token has been used" }, CriticalError = true };

            if (storedRefreshToken.JwtId != jti)
                return new AuthenticationDto { Errors = new[] { "This refresh token does not match this JWT" }, CriticalError = true };

            storedRefreshToken.Used = true;
            _context.RefreshTokens.Update(storedRefreshToken);
            await _context.SaveChangesAsync();

            var user = await _userManager.FindByIdAsync(validatedToken.Claims
                .Single(x => x.Type == "userId").Value);

            return await GenerateAuthenticationResultForUserAsync(user);
        }

        private ClaimsPrincipal GetPrincipalFromToken(string token)
        {
            var tokenHandler = new JwtSecurityTokenHandler();

            try
            {
                var tokenValidationParameters = _tokenValidationParameters.Clone();
                tokenValidationParameters.ValidateLifetime = false;
                var principal = tokenHandler.ValidateToken(token, tokenValidationParameters, out var validatedToken);
                if (!IsJwtWithValidSecurityAlgorithm(validatedToken))
                {
                    return null;
                }

                return principal;
            }
            catch
            {
                return null;
            }
        }

        private bool IsJwtWithValidSecurityAlgorithm(SecurityToken validatedToken)
        {
            return validatedToken is JwtSecurityToken jwtSecurityToken &&
                   jwtSecurityToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256,
                       StringComparison.InvariantCultureIgnoreCase);
        }

        private async Task<AuthenticationDto> GenerateAuthenticationResultForUserAsync(AppUser user)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_jwtSettings.Secret);

            var claims = new List<Claim>
            {
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim("userId", user.Id),
                new Claim(JwtRegisteredClaimNames.Email, user.Email)
            };

            var userClaims = await _userManager.GetClaimsAsync(user);
            claims.AddRange(userClaims);

            var userRoles = await _userManager.GetRolesAsync(user);
            foreach (var userRole in userRoles)
            {
                claims.Add(new Claim(ClaimTypes.Role, userRole));
                var role = await _roleManager.FindByNameAsync(userRole);
                if (role is null) continue;
                var roleClaims = await _roleManager.GetClaimsAsync(role);

                foreach (var roleClaim in roleClaims)
                {
                    if (claims.Contains(roleClaim))
                        continue;

                    claims.Add(roleClaim);
                }
            }

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.UtcNow.Add(_jwtSettings.TokenLifetime),
                SigningCredentials =
                    new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);

            var refreshToken = new RefreshToken
            {
                JwtId = token.Id,
                UserId = user.Id,
                CreationDate = DateTime.UtcNow,
                ExpiryDate = DateTime.UtcNow.AddMonths(6)
            };

            await _context.RefreshTokens.AddAsync(refreshToken);
            await _context.SaveChangesAsync();

            return new AuthenticationDto
            {
                Success = true,
                Token = tokenHandler.WriteToken(token),
                RefreshToken = refreshToken.Token
            };
        }
    }
}
