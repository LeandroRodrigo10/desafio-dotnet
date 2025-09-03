using System;

namespace Ambev.DeveloperEvaluation.WebApi.Features.Sales.UpdateSale
{
    /// <summary>
    /// DTO de resposta para atualização de venda.
    /// </summary>
    public sealed class UpdateSaleResponse
    {
        public Guid Id { get; set; }
        public Guid? CustomerId { get; set; }
        public decimal? Total { get; set; }
        public string? Status { get; set; }
    }
}
