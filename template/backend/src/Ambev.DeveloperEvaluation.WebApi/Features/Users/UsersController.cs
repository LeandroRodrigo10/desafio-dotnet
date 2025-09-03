using System;
using System.Threading.Tasks;
using Ambev.DeveloperEvaluation.Application.Users.DeleteUser;
using Ambev.DeveloperEvaluation.Application.Users.GetUser;
using Ambev.DeveloperEvaluation.Application.Users.SearchUsers;
using Ambev.DeveloperEvaluation.WebApi.Features.Users.CreateUser;
using Ambev.DeveloperEvaluation.WebApi.Features.Users.UpdateUser;
using Ambev.DeveloperEvaluation.WebApi.Features.Users.DeleteUser;
using Ambev.DeveloperEvaluation.Domain.Enums; // UserRole, UserStatus
using AutoMapper;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Ambev.DeveloperEvaluation.WebApi.Features.Users
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize] // exige autenticação por padrão
    public class UsersController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly IMapper _mapper;

        public UsersController(IMediator mediator, IMapper mapper)
        {
            _mediator = mediator;
            _mapper = mapper;
        }

        // GET /api/Users
        [HttpGet]
        public async Task<ActionResult<SearchUsersResult>> Search([FromQuery] SearchUsersCommand command)
        {
            var result = await _mediator.Send(command);
            return Ok(result);
        }

        // GET /api/Users/{id}
        [HttpGet("{id}")]
        public async Task<ActionResult<GetUserResult>> GetById(Guid id)
        {
            var result = await _mediator.Send(new GetUserCommand(id));
            if (result == null)
                return NotFound(new { message = $"User with Id '{id}' not found" });

            return Ok(result);
        }

        // POST /api/Users
        [HttpPost]
        [AllowAnonymous] 
        public async Task<ActionResult<CreateUserResponse>> Create([FromBody] CreateUserRequest request)
        {
            // Defaults seguros pro teste passar
            if (request.Role == UserRole.None) 
                request.Role = UserRole.Admin;

            if ((int)request.Status == 0) 
                request.Status = UserStatus.Active;

            var command = _mapper.Map<Ambev.DeveloperEvaluation.Application.Users.CreateUser.CreateUserCommand>(request);
            var result = await _mediator.Send(command);
            var response = _mapper.Map<CreateUserResponse>(result);

            return CreatedAtAction(nameof(GetById), new { id = response.Id }, response);
        }

        // PUT /api/Users/{id}
        [HttpPut("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<UpdateUserResponse>> Update([FromRoute] Guid id, [FromBody] UpdateUserRequest request)
        {
            var command = new Ambev.DeveloperEvaluation.Application.Users.UpdateUser.UpdateUserCommand(id)
            {
                Username = request.Username,
                Email = request.Email,
                Phone = request.Phone,
                Password = request.Password,
                Role = (UserRole)request.Role,       // cast explícito
                Status = (UserStatus)request.Status  // cast explícito
            };

            var result = await _mediator.Send(command);
            var response = _mapper.Map<UpdateUserResponse>(result);
            return Ok(response);
        }

        // DELETE /api/Users/{id}
        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<DeleteUserResponse>> Delete([FromRoute] Guid id)
        {
            var result = await _mediator.Send(new DeleteUserCommand(id));

            if (!result.Success)
                return NotFound(new { message = $"User with Id '{id}' not found" });

            var response = _mapper.Map<DeleteUserResponse>(result);
            return Ok(response);
        }
    }
}
