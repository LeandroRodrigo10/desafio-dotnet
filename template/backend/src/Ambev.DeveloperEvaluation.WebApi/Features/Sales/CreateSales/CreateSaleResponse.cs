using System;

namespace Ambev.DeveloperEvaluation.WebApi.Features.Sales.CreateSale
{
    /// <summary>
    /// DTO de resposta para criação de venda.
    /// </summary>
    public sealed class CreateSaleResponse
    {
        public Guid Id { get; set; }
        public Guid? CustomerId { get; set; }
        public decimal? Total { get; set; }
        public string? Status { get; set; }
    }
}
