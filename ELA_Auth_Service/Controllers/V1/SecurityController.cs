using System.Linq;
using System.Threading.Tasks;
using ELA_Auth_Service.Contracts.V1;
using ELA_Auth_Service.Contracts.V1.Requests.Identity.Email;
using ELA_Auth_Service.Contracts.V1.Requests.Identity.Password;
using ELA_Auth_Service.Contracts.V1.Responses.Identity.Email;
using ELA_Auth_Service.Contracts.V1.Responses.Identity.Password;
using ELA_Auth_Service.Domain.Entities;
using ELA_Auth_Service.Services.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ELA_Auth_Service.Controllers.V1
{
    [Produces("application/json")]
    public class SecurityController : Controller
    {
        private readonly ISecurityService _securityService;

        public SecurityController(ISecurityService securityService) => _securityService = securityService;


        /// <summary>
        /// Password reset request endpoint, sending link to the provided email
        /// </summary>
        /// <response code="204">Success-full password reset request sending link to the provided email</response>
        /// <response code="400">Failed password reset request returns a list of errors, an error can be critical</response>
        [ProducesResponseType(typeof(PasswordResetRequestFailedResponse), 400)]
        [HttpPost(ApiRoutes.UserManager.PasswordResetRequest)]
        public async Task<IActionResult> PasswordResetRequest([FromBody] PasswordResetRequest request)
        {
            if (request.Email is null)
                return BadRequest(new PasswordResetRequestFailedResponse
                {
                    Errors = new[] { "Please make sure you send correct request, field can't be null" },
                    CriticalError = true
                });

            if (!ModelState.IsValid)
                return BadRequest(new PasswordResetRequestFailedResponse
                {
                    Errors = ModelState.Values.SelectMany(x => x.Errors.Select(xx => xx.ErrorMessage))
                });

            var requestResponse = await _securityService.PasswordResetRequestAsync(request.Email);

            if (!requestResponse.Success)
                return BadRequest(new PasswordResetRequestFailedResponse { Errors = requestResponse.Errors, CriticalError = requestResponse.CriticalError });

            return NoContent();
        }


        /// <summary>
        /// Update password endpoint returns status code or error object
        /// </summary>
        /// <response code="204">Success-full password update setting new password for user</response>
        /// <response code="400">Failed password update, returns a list of errors, an error can be critical</response>
        [ProducesResponseType(typeof(PasswordUpdateFailedResponse), 400)]
        [HttpPut(ApiRoutes.UserManager.UpdatePassword)]
        public async Task<IActionResult> UpdatePassword([FromBody] PasswordUpdateRequest request)
        {
            if (request.UserId is null || request.Password is null || request.Token is null)
            {
                return BadRequest(new PasswordUpdateFailedResponse
                {
                    Errors = new[] { "Please make sure you send correct request, field can't be null" },
                    CriticalError = true
                });
            }

            var resetResult = await _securityService.PasswordUpdateAsync(request.UserId, request.Password, request.Token);

            if (!resetResult.Success)
                return BadRequest(new PasswordUpdateFailedResponse { Errors = resetResult.Errors, CriticalError = resetResult.CriticalError });

            return NoContent();
        }


        /// <summary>
        /// Email confirmation request endpoint, sending link to the provided email
        /// </summary>
        /// <response code="204">Sending link to the provided email</response>
        /// <response code="400">Return list of errors, an error can be critical</response>
        [ProducesResponseType(typeof(EmailConfirmationResponse), 400)]
        [HttpPost(ApiRoutes.UserManager.EmailConfirmationRequest)]
        public async Task<IActionResult> EmailConfirmationRequest([FromBody] EmailConfirmationRequest request)
        {
            if (request.Email is null)
                return BadRequest(new EmailConfirmationResponse
                {
                    Errors = new[] { "Please make sure you send correct request, field can't be null" },
                    CriticalError = true
                });

            var emailConfirmationRequestLink = await _securityService.SendEmailConfirmationRequestAsync(request.Email);

            if (!emailConfirmationRequestLink.Success)
                return BadRequest(new EmailConfirmationResponse
                {
                    Errors = emailConfirmationRequestLink.Errors,
                    CriticalError = emailConfirmationRequestLink.CriticalError
                });

            return NoContent();
        }


        /// <summary>
        /// Email confirmation endpoint, confirming user e-mail
        /// </summary>
        /// <response code="204">Email was successfully confirmed</response>
        /// <response code="400">Return list of errors, an error can be critical</response>
        [ProducesResponseType(typeof(EmailConfirmationResponse), 400)]
        [HttpPut(ApiRoutes.UserManager.ConfirmEmail)]
        public async Task<IActionResult> ConfirmEmail([FromBody] ConfirmEmailRequest request)
        {
            if (request.UserId is null || request.Token is null)
                return BadRequest(new EmailConfirmationResponse
                {
                    Errors = new[] { "Please make sure you send correct request, field  can't be null" },
                    CriticalError = true
                });

            var emailConfirmationResult = await _securityService.ConfirmEmailAsync(request.UserId, request.Token);

            if(!emailConfirmationResult.Success)
                return BadRequest(new EmailConfirmationResponse
                {
                    Errors = emailConfirmationResult.Errors,
                    CriticalError = emailConfirmationResult.CriticalError
                });

            return NoContent();
        }
    }
}