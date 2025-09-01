using System.Threading;
using System.Threading.Tasks;
using Ambev.DeveloperEvaluation.Domain.Repositories;
using MediatR;

namespace Ambev.DeveloperEvaluation.Application.Users.DeleteUser
{
    public class DeleteUserHandler : IRequestHandler<DeleteUserCommand, DeleteUserResponse>
    {
        private readonly IUserRepository _userRepository;

        public DeleteUserHandler(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        public async Task<DeleteUserResponse> Handle(DeleteUserCommand request, CancellationToken cancellationToken)
        {
            var user = await _userRepository.GetByIdAsync(request.Id, cancellationToken);
            if (user is null)
                return new DeleteUserResponse { Success = false };

            await _userRepository.DeleteAsync(request.Id, cancellationToken);

            return new DeleteUserResponse { Success = true };
        }
    }
}
