#nullable enable

using System;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using MediatR;
using Ambev.DeveloperEvaluation.Domain.Entities;
using Ambev.DeveloperEvaluation.Domain.Repositories;

namespace Ambev.DeveloperEvaluation.Application.Users.GetUser
{
    /// <summary>
    /// Handles the GetUserCommand request using the provided user Id.
    /// </summary>
    public sealed class GetUserHandler : IRequestHandler<GetUserCommand, GetUserResult>
    {
        private readonly IUserRepository _userRepository;
        private readonly IMapper _mapper;

        public GetUserHandler(IUserRepository userRepository, IMapper mapper)
        {
            _userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        public async Task<GetUserResult> Handle(GetUserCommand request, CancellationToken cancellationToken)
        {
            var user = await _userRepository.GetByIdAsync(request.Id, cancellationToken);

            if (user is null)
                throw new KeyNotFoundException($"User with Id {request.Id} not found.");

            return _mapper.Map<GetUserResult>(user);
        }
    }
}
