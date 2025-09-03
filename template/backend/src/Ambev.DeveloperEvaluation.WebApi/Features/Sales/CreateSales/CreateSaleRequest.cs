using System;

namespace Ambev.DeveloperEvaluation.WebApi.Features.Sales.CreateSale
{
    /// <summary>
    /// Request payload para criação de uma venda.
 
    public sealed class CreateSaleRequest
    {
        /// <summary>Identificador do cliente 
        public Guid? CustomerId { get; set; }

        /// <summary>Valor total da venda
        public decimal? Total { get; set; }

        /// <summary>Status textual 
        public string? Status { get; set; }
    }
}
