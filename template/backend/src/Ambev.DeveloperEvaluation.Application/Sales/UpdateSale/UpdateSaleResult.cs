using System;
using System.Collections.Generic;

namespace Ambev.DeveloperEvaluation.Application.Sales.UpdateSale
{
    public class UpdateSaleResult
    {
        public Guid Id { get; set; }
        public string Number { get; set; } = default!;
        public DateTime Date { get; set; }
        public string Customer { get; set; } = default!;
        public string Branch { get; set; } = default!;
        public string Status { get; set; } = default!;
        public decimal TotalAmount { get; set; }

        public List<UpdateSaleItemResult> Items { get; set; } = new();

        public class UpdateSaleItemResult
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
