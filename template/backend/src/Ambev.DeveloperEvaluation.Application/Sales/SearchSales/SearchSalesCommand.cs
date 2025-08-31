using System;
using MediatR;
using Ambev.DeveloperEvaluation.Domain.Enums;

namespace Ambev.DeveloperEvaluation.Application.Sales.SearchSales
{
    public class SearchSalesCommand : IRequest<SearchSalesResult>
    {
        // Paginação
        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 10;

        // Filtros
        public string? Q { get; set; }
        public string? Customer { get; set; }
        public string? Branch { get; set; }
        public SaleStatus? Status { get; set; }
        public DateTime? DateFrom { get; set; }
        public DateTime? DateTo { get; set; }

        // Ordenação: ex. "date", "-date", "customer", "-totalAmount"
        public string? Sort { get; set; }
    }
}
