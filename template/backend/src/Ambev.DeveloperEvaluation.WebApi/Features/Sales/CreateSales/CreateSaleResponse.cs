using System;
using System.Collections.Generic;

namespace Ambev.DeveloperEvaluation.WebApi.Features.Sales.CreateSale
{
    /// <summary>
    /// DTO de resposta para criação de venda.
    /// </summary>
    public sealed class CreateSaleResponse
    {
        public Guid Id { get; set; }

        /// <summary>Número da venda (ex.: S-0003)</summary>
        public string Number { get; set; } = default!;

        /// <summary>Data/hora da venda</summary>
        public DateTime Date { get; set; }

        /// <summary>Nome/identificação do cliente</summary>
        public string Customer { get; set; } = default!;

        /// <summary>Filial onde a venda foi realizada</summary>
        public string Branch { get; set; } = default!;

        /// <summary>Status atual da venda (Active/Cancelled)</summary>
        public string Status { get; set; } = default!;

        /// <summary>Valor total da venda</summary>
        public decimal Total { get; set; }

        /// <summary>Itens da venda</summary>
        public List<SaleItemResponse> Items { get; set; } = new();

        public sealed class SaleItemResponse
        {
            public Guid Id { get; set; }
            public string Sku { get; set; } = default!;
            public string Name { get; set; } = default!;
            public int Quantity { get; set; }
            public decimal UnitPrice { get; set; }
            public decimal Discount { get; set; }
            public decimal Total { get; set; }
        }
    }
}
