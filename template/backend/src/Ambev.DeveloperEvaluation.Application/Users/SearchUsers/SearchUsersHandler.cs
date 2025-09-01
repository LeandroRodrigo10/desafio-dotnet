using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Ambev.DeveloperEvaluation.Domain.Repositories;
using AutoMapper;
using MediatR;

namespace Ambev.DeveloperEvaluation.Application.Users.SearchUsers
{
    public class SearchUsersHandler : IRequestHandler<SearchUsersCommand, SearchUsersResult>
    {
        private readonly IUserRepository _repository;
        private readonly IMapper _mapper;

        public SearchUsersHandler(IUserRepository repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        public async Task<SearchUsersResult> Handle(SearchUsersCommand request, CancellationToken cancellationToken)
        {
            var (items, total) = await _repository.SearchAsync(
                request.Page,
                request.PageSize,
                request.Q,
                request.Email,
                request.Status,
                request.Role,
                request.Sort,
                cancellationToken
            );

            return new SearchUsersResult
            {
                Page = request.Page,
                PageSize = request.PageSize,
                Total = total,
                Items = _mapper.Map<List<SearchUsersResult.UserSummary>>(items)
            };
        }
    }
}
