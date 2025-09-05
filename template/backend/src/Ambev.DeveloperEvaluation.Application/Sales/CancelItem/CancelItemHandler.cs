using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Ambev.DeveloperEvaluation.Domain.Repositories;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Ambev.DeveloperEvaluation.Application.Sales.CancelItem
{
    // Command
    public sealed class CancelItemCommand : IRequest<CancelItemResult>
    {
        public Guid SaleId { get; }
        public Guid ItemId { get; }

        public CancelItemCommand(Guid saleId, Guid itemId)
        {
            SaleId = saleId;
            ItemId = itemId;
        }
    }

    // Result
    public sealed record CancelItemResult(
        Guid SaleId,
        Guid ItemId,
        string Number,
        decimal TotalAmount
    );

    // Handler
    public sealed class CancelItemHandler : IRequestHandler<CancelItemCommand, CancelItemResult>
    {
        private readonly ISaleRepository _repository;
        private readonly ILogger<CancelItemHandler> _logger;

        public CancelItemHandler(ISaleRepository repository, ILogger<CancelItemHandler> logger)
        {
            _repository = repository;
            _logger = logger;
        }

        public async Task<CancelItemResult> Handle(CancelItemCommand request, CancellationToken cancellationToken)
        {
            var sale = await _repository.GetByIdAsync(request.SaleId, cancellationToken);
            if (sale is null)
                throw new KeyNotFoundException("Sale not found");

            // Usa a regra atual: remove o item (preserva a venda e recalcula total)
            sale.RemoveItem(request.ItemId);

            await _repository.UpdateAsync(sale, cancellationToken);

            // Evento diferencial: ItemCancelled
            _logger.LogInformation(
                "DomainEvent=ItemCancelled SaleId={SaleId} ItemId={ItemId} Number={Number} TotalAmount={TotalAmount}",
                sale.Id,
                request.ItemId,
                sale.Number,
                sale.TotalAmount
            );

            return new CancelItemResult(sale.Id, request.ItemId, sale.Number, sale.TotalAmount);
        }
    }
}
