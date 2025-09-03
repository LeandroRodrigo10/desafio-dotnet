using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
// Alias para usar as classes do Application
using AppAuth = Ambev.DeveloperEvaluation.Application.Auth.AuthenticateUser;

namespace Ambev.DeveloperEvaluation.WebApi.Features.Auth
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IMediator _mediator;

        public AuthController(IMediator mediator) => _mediator = mediator;

        [AllowAnonymous]
        [HttpPost]
        public async Task<IActionResult> AuthenticateUser(
            [FromBody] AppAuth.AuthenticateUserRequest request,
            CancellationToken ct)
        {
            var result = await _mediator.Send(request, ct);

            if (result is null || string.IsNullOrWhiteSpace(result.Token) || result.Token.Split('.').Length != 3)
                return Unauthorized(new { error = "Token inválido gerado pelo servidor." });

            return Ok(new { data = new { token = result.Token } });
        }
    }
}
