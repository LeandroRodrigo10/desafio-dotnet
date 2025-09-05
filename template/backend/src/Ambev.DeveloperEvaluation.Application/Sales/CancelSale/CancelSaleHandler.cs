using System;
using System.Linq;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Ambev.DeveloperEvaluation.Domain.Repositories;
using Microsoft.Extensions.Logging;

namespace Ambev.DeveloperEvaluation.Application.Sales.CancelSale
{
    public sealed class CancelSaleHandler : IRequestHandler<CancelSaleCommand, CancelSaleResult>
    {
        private readonly ISaleRepository _repository;
        private readonly ILogger<CancelSaleHandler> _logger;

        public CancelSaleHandler(ISaleRepository repository, ILogger<CancelSaleHandler> logger)
        {
            _repository = repository;
            _logger = logger;
        }

        public async Task<CancelSaleResult> Handle(CancelSaleCommand request, CancellationToken cancellationToken)
        {
            var sale = await _repository.GetByIdAsync(request.Id, cancellationToken);
            if (sale is null)
                throw new KeyNotFoundException("Sale not found");

            sale.Cancel();

            await _repository.UpdateAsync(sale, cancellationToken);

            // Evento (diferencial): SaleCancelled
            _logger.LogInformation(
                "DomainEvent=SaleCancelled SaleId={SaleId} Number={Number} Status={Status} Items={Items} TotalAmount={TotalAmount}",
                sale.Id,
                sale.Number,
                sale.Status.ToString(),
                sale.Items.Select(i => new { i.Sku, i.Quantity, i.Total }),
                sale.TotalAmount
            );

            return new CancelSaleResult(sale.Id, sale.Number, sale.Status.ToString(), sale.TotalAmount);
        }
    }
}
