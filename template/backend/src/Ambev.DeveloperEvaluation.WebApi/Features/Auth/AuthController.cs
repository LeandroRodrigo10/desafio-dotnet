using System.Threading;
using System.Threading.Tasks;
using System.Linq;
using Ambev.DeveloperEvaluation.Application.Auth.AuthenticateUser;
using Ambev.DeveloperEvaluation.WebApi.Common;
using Ambev.DeveloperEvaluation.WebApi.Features.Auth.AuthenticateUserFeature;
using Ambev.DeveloperEvaluation.Common.Validation; // <-- para ValidationErrorDetail
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using FluentValidation;

namespace Ambev.DeveloperEvaluation.WebApi.Features.Auth
{
    /// <summary>
    /// Controller for authentication operations
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : BaseController
    {
        private readonly IMediator _mediator;

        /// <summary>
        /// Initializes a new instance of AuthController
        /// </summary>
        public AuthController(IMediator mediator)
        {
            _mediator = mediator;
        }

        /// <summary>
        /// Authenticates a user with email and password and returns a JWT token.
        /// </summary>
        [HttpPost]
        [AllowAnonymous]
        [ProducesResponseType(typeof(ApiResponseWithData<AuthenticateUserResponse>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> AuthenticateUser(
            [FromBody] AuthenticateUserRequest request,
            CancellationToken cancellationToken)
        {
            if (request is null || string.IsNullOrWhiteSpace(request.Email) || string.IsNullOrWhiteSpace(request.Password))
            {
                return BadRequest(new ApiResponse
                {
                    Success = false,
                    Message = "Email and password are required"
                });
            }

            try
            {
                var command = new AuthenticateUserCommand
                {
                    Email = request.Email,
                    Password = request.Password
                };

                var result = await _mediator.Send(command, cancellationToken);

                if (result is null || string.IsNullOrWhiteSpace(result.Token))
                {
                    return Unauthorized(new ApiResponse
                    {
                        Success = false,
                        Message = "Invalid email or password"
                    });
                }

                var response = new AuthenticateUserResponse
                {
                    Token = result.Token,
                    Email = request.Email,
                    Name = result.Name ?? string.Empty,
                    Role = result.Role ?? string.Empty
                };

                return Ok(new ApiResponseWithData<AuthenticateUserResponse>
                {
                    Success = true,
                    Message = "User authenticated successfully",
                    Data = response
                });
            }
            catch (ValidationException ex)
            {
                return BadRequest(new ApiResponse
                {
                    Success = false,
                    Message = "Validation Failed",
                    // Usa o cast já utilizado no domínio: (ValidationErrorDetail)e
                    Errors = (ex.Errors?.Select(e => (ValidationErrorDetail)e))
                             ?? Enumerable.Empty<ValidationErrorDetail>()
                });
            }
            catch (UnauthorizedAccessException)
            {
                return Unauthorized(new ApiResponse
                {
                    Success = false,
                    Message = "Invalid email or password"
                });
            }
        }
    }
}
