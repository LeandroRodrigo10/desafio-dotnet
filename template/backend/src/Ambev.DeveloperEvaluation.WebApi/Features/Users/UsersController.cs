using System;
using System.Threading.Tasks;
using Ambev.DeveloperEvaluation.Application.Users.DeleteUser;
using Ambev.DeveloperEvaluation.Application.Users.GetUser;
using Ambev.DeveloperEvaluation.Application.Users.SearchUsers;
using Ambev.DeveloperEvaluation.WebApi.Features.Users.CreateUser;
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
            // Mapeia o request WebApi -> Command da Application
            var command = _mapper.Map<Ambev.DeveloperEvaluation.Application.Users.CreateUser.CreateUserCommand>(request);

            var result = await _mediator.Send(command);

            // Mapeia o resultado da Application -> Response da WebApi
            var response = _mapper.Map<CreateUserResponse>(result);

            return CreatedAtAction(nameof(GetById), new { id = response.Id }, response);
        }

        /// <summary>
        /// Delete a user
        /// </summary>
        [HttpDelete("{id}")]
        public async Task<ActionResult<DeleteUserResponse>> Delete(Guid id)
        {
            var result = await _mediator.Send(new DeleteUserCommand(id));
            if (!result.Success)
                return NotFound(new { message = $"User with Id '{id}' not found" });

            return Ok(result);
        }
    }
}
