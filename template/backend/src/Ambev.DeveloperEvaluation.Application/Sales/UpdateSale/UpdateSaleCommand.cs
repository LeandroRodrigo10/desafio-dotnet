using System;
using System.Collections.Generic;
using MediatR;

namespace Ambev.DeveloperEvaluation.Application.Sales.UpdateSale
{
    public class UpdateSaleCommand : IRequest<UpdateSaleResult>
    {
        public Guid Id { get; set; }

        public string Number { get; set; } = default!;
        public DateTime Date { get; set; }
        public string Customer { get; set; } = default!;
        public string Branch { get; set; } = default!;

        // Substitui os itens atuais pelos enviados
        public List<UpdateSaleItemDto> Items { get; set; } = new();

        public class UpdateSaleItemDto
        {
            public Guid? Id { get; set; } // opcional para rastrear item existente
            public string Sku { get; set; } = default!;
            public string Name { get; set; } = default!;
            public int Quantity { get; set; }
            public decimal UnitPrice { get; set; }
            public decimal Discount { get; set; }
        }
    }
}
