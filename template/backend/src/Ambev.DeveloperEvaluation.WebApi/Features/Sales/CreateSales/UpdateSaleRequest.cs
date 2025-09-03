using System;

namespace Ambev.DeveloperEvaluation.WebApi.Features.Sales.UpdateSale
{
    /// <summary>
    /// Request payload para atualização de uma venda existente.
    /// </summary>
    public sealed class UpdateSaleRequest
    {
        /// <summary>Identificador do cliente 
        public Guid? CustomerId { get; set; }

        /// <summary>Valor total da venda
        public decimal? Total { get; set; }

        /// <summary>Status textual 
        public string? Status { get; set; }
    }
}
