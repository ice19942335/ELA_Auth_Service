using System.Linq;
using System.Threading.Tasks;
using ELA_Auth_Service.Contracts.V1;
using ELA_Auth_Service.Contracts.V1.Requests.Authentication.Auth;
using ELA_Auth_Service.Contracts.V1.Responses.Authentication.Auth;
using ELA_Auth_Service.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace ELA_Auth_Service.Controllers.V1
{
    [Produces("application/json")]
    public class AuthenticationController : Controller
    {
        private readonly IIdentityService _identityService;

        public AuthenticationController(IIdentityService identityService) => _identityService = identityService;

        /// <summary>
        /// Registration endpoint. Registering user in the system and generating pair Access-Refresh Token
        /// </summary>
        /// <response code="200">Success-full registration returns pair Access-Refresh Token</response>
        /// <response code="400">Failed registration returns list of errors, error can be Critical</response>
        [HttpPost(ApiRoutes.Authentication.Register)]
        [ProducesResponseType(typeof(AuthSuccessResponse), 200)]
        [ProducesResponseType(typeof(AuthFailedResponse), 400)]
        public async Task<IActionResult> Register([FromBody] UserRegistrationRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest(new AuthFailedResponse { Errors = ModelState.Values.SelectMany(x => x.Errors.Select(xx => xx.ErrorMessage)) });

            var authResponse = await _identityService.RegisterAsync(request);

            if (!authResponse.Success)
                return BadRequest(new AuthFailedResponse { Errors = authResponse.Errors });

            return Ok(new AuthSuccessResponse
            {
                Token = authResponse.Token,
                RefreshToken = authResponse.RefreshToken
            });
        }

        /// <summary>
        /// Login endpoint. If provided credentials are correct, then Generating new pair Access-Refresh Token
        /// </summary>
        /// <response code="200">Success-full login returns pair Access-Refresh Token</response>
        /// <response code="400">Failed Login returns list of errors</response>
        [HttpPost(ApiRoutes.Authentication.Login)]
        [ProducesResponseType(typeof(AuthSuccessResponse), 200)]
        [ProducesResponseType(typeof(AuthFailedResponse), 400)]
        public async Task<IActionResult> Login([FromBody] UserLoginRequest request)
        {
            var authResponse = await _identityService.LoginAsync(request.Email, request.Password);

            if (!authResponse.Success)
                return BadRequest(new AuthFailedResponse { Errors = authResponse.Errors });

            return Ok(new AuthSuccessResponse
            {
                Token = authResponse.Token,
                RefreshToken = authResponse.RefreshToken
            });
        }

        /// <summary>
        /// Refresh endpoint.  If the provided pair is valid, then generating a new pair Access-Refresh Token
        /// </summary>
        /// <response code="200">Success-full refresh returns pair Access-Refresh Token</response>
        /// <response code="400">Failed refresh returns list of errors, error can be Critical</response>
        [HttpPost(ApiRoutes.Authentication.Refresh)]
        [ProducesResponseType(typeof(AuthSuccessResponse), 200)]
        [ProducesResponseType(typeof(AuthFailedResponse), 400)]
        public async Task<IActionResult> RefreshToken([FromBody] RefreshTokenRequest request)
        {
            if (request.Token is null || request.RefreshToken is null)
                return BadRequest(new AuthFailedResponse
                {
                    Errors = new[] { "Please make sure you send correct request, field can't be null" },
                    CriticalError = true
                });

            var authResponse = await _identityService.RefreshTokenAsync(request.Token, request.RefreshToken);

            if (!authResponse.Success)
                return BadRequest(new AuthFailedResponse
                {
                    Errors = authResponse.Errors,
                    CriticalError = authResponse.CriticalError
                });

            return Ok(new AuthSuccessResponse
            {
                Token = authResponse.Token,
                RefreshToken = authResponse.RefreshToken
            });
        }
    }
}