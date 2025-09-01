using System;
using System.Threading.Tasks;
using Ambev.DeveloperEvaluation.Application.Users.DeleteUser;
using Ambev.DeveloperEvaluation.Application.Users.GetUser;
using Ambev.DeveloperEvaluation.Application.Users.SearchUsers;
using Ambev.DeveloperEvaluation.WebApi.Features.Users.CreateUser;
using Ambev.DeveloperEvaluation.WebApi.Features.Users.UpdateUser;
using Ambev.DeveloperEvaluation.WebApi.Features.Users.DeleteUser;

using AutoMapper;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Ambev.DeveloperEvaluation.WebApi.Features.Users
{
    [ApiController]
    [Route("api/[controller]")]
    public class UsersController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly IMapper _mapper;

        public UsersController(IMediator mediator, IMapper mapper)
        {
            _mediator = mediator;
            _mapper = mapper;
        }

        /// <summary>
        /// Search users with pagination and filters
        /// </summary>
        [HttpGet]
        public async Task<ActionResult<SearchUsersResult>> Search([FromQuery] SearchUsersCommand command)
        {
            var result = await _mediator.Send(command);
            return Ok(result);
        }

        /// <summary>
        /// Get user by Id
        /// </summary>
        [HttpGet("{id}")]
        public async Task<ActionResult<GetUserResult>> GetById(Guid id)
        {
            var result = await _mediator.Send(new GetUserCommand(id));
            if (result == null)
                return NotFound(new { message = $"User with Id '{id}' not found" });

            return Ok(result);
        }

        /// <summary>
        /// Create a new user
        /// </summary>
        [HttpPost]
        public async Task<ActionResult<CreateUserResponse>> Create([FromBody] CreateUserRequest request)
        {
            var command = _mapper.Map<Ambev.DeveloperEvaluation.Application.Users.CreateUser.CreateUserCommand>(request);
            var result = await _mediator.Send(command);
            var response = _mapper.Map<CreateUserResponse>(result);

            return CreatedAtAction(nameof(GetById), new { id = response.Id }, response);
        }

        /// <summary>
        /// Update an existing user
        /// </summary>
        [HttpPut("{id}")]
        public async Task<ActionResult<UpdateUserResponse>> Update([FromRoute] Guid id, [FromBody] UpdateUserRequest request)
        {
            var command = new Ambev.DeveloperEvaluation.Application.Users.UpdateUser.UpdateUserCommand(id)
            {
                Username = request.Username,
                Email = request.Email,
                Phone = request.Phone,
                Password = request.Password,
                Role = (Ambev.DeveloperEvaluation.Domain.Enums.UserRole)request.Role,
                Status = (Ambev.DeveloperEvaluation.Domain.Enums.UserStatus)request.Status
            };

            var result = await _mediator.Send(command);
            var response = _mapper.Map<UpdateUserResponse>(result);
            return Ok(response);
        }

        /// <summary>
        /// Delete a user
        /// </summary>
        [HttpDelete("{id}")]
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
