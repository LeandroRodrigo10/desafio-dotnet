using System.Threading;
using System.Threading.Tasks;
using Ambev.DeveloperEvaluation.Domain.Repositories;
using AutoMapper;
using MediatR;

namespace Ambev.DeveloperEvaluation.Application.Sales.SearchSales
{
    public class SearchSalesHandler : IRequestHandler<SearchSalesCommand, SearchSalesResult>
    {
        private readonly ISaleRepository _repository;
        private readonly IMapper _mapper;

        public SearchSalesHandler(ISaleRepository repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        public async Task<SearchSalesResult> Handle(SearchSalesCommand request, CancellationToken cancellationToken)
        {
            var (items, total) = await _repository.SearchAsync(
                request.Page,
                request.PageSize,
                request.Q,
                request.Customer,
                request.Branch,
                request.Status,
                request.DateFrom,
                request.DateTo,
                request.Sort,
                cancellationToken
            );

            return new SearchSalesResult
            {
                Page = request.Page,
                PageSize = request.PageSize,
                Total = total,
                Items = _mapper.Map<List<SearchSalesResult.SaleSummary>>(items)
            };
        }
    }
}
