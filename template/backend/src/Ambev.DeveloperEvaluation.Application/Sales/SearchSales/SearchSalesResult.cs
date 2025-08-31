using System;
using System.Collections.Generic;

namespace Ambev.DeveloperEvaluation.Application.Sales.SearchSales
{
    public class SearchSalesResult
    {
        public int Page { get; set; }
        public int PageSize { get; set; }
        public int Total { get; set; }
        public List<SaleSummary> Items { get; set; } = new();

        public class SaleSummary
        {
            public Guid Id { get; set; }
            public string Number { get; set; } = default!;
            public DateTime Date { get; set; }
            public string Customer { get; set; } = default!;
            public string Branch { get; set; } = default!;
            public string Status { get; set; } = default!;
            public decimal TotalAmount { get; set; }
        }
    }
}
