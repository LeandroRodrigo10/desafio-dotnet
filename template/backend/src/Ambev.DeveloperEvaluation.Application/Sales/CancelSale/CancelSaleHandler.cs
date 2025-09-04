using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Ambev.DeveloperEvaluation.Domain.Repositories;

namespace Ambev.DeveloperEvaluation.Application.Sales.CancelSale
{
    public sealed class CancelSaleHandler : IRequestHandler<CancelSaleCommand, CancelSaleResult>
    {
        private readonly ISaleRepository _repository;

        public CancelSaleHandler(ISaleRepository repository)
        {
            _repository = repository;
        }

        public async Task<CancelSaleResult> Handle(CancelSaleCommand request, CancellationToken cancellationToken)
        {
            var sale = await _repository.GetByIdAsync(request.Id, cancellationToken);
            if (sale is null)
                throw new KeyNotFoundException("Sale not found");

            sale.Cancel();

            await _repository.UpdateAsync(sale, cancellationToken);

            return new CancelSaleResult(sale.Id, sale.Number, sale.Status.ToString(), sale.TotalAmount);
        }
    }
}
