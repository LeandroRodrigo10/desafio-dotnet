using System;
using MediatR;

namespace Ambev.DeveloperEvaluation.Application.Sales.CancelSale
{
    public sealed class CancelSaleCommand : IRequest<CancelSaleResult>
    {
        public Guid Id { get; }

        public CancelSaleCommand(Guid id)
        {
            Id = id;
        }
    }

    public sealed record CancelSaleResult(
        Guid Id,
        string Number,
        string Status,
        decimal TotalAmount
    );
}
