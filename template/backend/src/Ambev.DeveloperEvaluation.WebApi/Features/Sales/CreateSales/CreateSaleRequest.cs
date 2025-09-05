using System;
using System.Collections.Generic;

namespace Ambev.DeveloperEvaluation.WebApi.Features.Sales.CreateSale
{
    /// <summary>
    /// Payload para criação de uma Venda.
    /// </summary>
    public sealed class CreateSaleRequest
    {
        /// <summary>Número da venda (ex.: S-0001)</summary>
        public string Number { get; set; } = default!;

        /// <summary>Data/hora da venda</summary>
        public DateTime Date { get; set; }

        /// <summary>Nome/identificação do cliente</summary>
        public string Customer { get; set; } = default!;

        /// <summary>Filial onde a venda foi realizada</summary>
        public string Branch { get; set; } = default!

        ;/// <summary>Itens da venda</summary>
        public List<CreateSaleItemRequest> Items { get; set; } = new();
    }

    /// <summary>
    /// Item da venda para criação.
    /// </summary>
    public sealed class CreateSaleItemRequest
    {
        /// <summary>SKU do produto</summary>
        public string Sku { get; set; } = default!;

        /// <summary>Nome/descrição do produto</summary>
        public string Name { get; set; } = default!;

        /// <summary>Quantidade (1–20)</summary>
        public int Quantity { get; set; }

        /// <summary>Preço unitário</summary>
        public decimal UnitPrice { get; set; }

        /// <summary>Desconto absoluto por unidade (será recalculado pelas regras de tier)</summary>
        public decimal Discount { get; set; } = 0m;
    }
}
