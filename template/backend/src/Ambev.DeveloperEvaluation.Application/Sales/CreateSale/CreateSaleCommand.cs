using System;
using System.Collections.Generic;
using MediatR;

namespace Ambev.DeveloperEvaluation.Application.Sales.CreateSale
{
    public class CreateSaleCommand : IRequest<CreateSaleResult>
    {
        public string Number { get; set; } = default!;
        public DateTime Date { get; set; }
        public string Customer { get; set; } = default!;
        public string Branch { get; set; } = default!;
        public List<CreateSaleItemDto> Items { get; set; } = new();

        public class CreateSaleItemDto
        {
            public string Sku { get; set; } = default!;
            public string Name { get; set; } = default!;
            public int Quantity { get; set; }
            public decimal UnitPrice { get; set; }
            public decimal Discount { get; set; }
        }
    }
}
