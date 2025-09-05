using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Ambev.DeveloperEvaluation.Domain.Repositories;
using Ambev.DeveloperEvaluation.Domain.Entities;
using AutoMapper;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Ambev.DeveloperEvaluation.Application.Sales.UpdateSale
{
    public class UpdateSaleHandler : IRequestHandler<UpdateSaleCommand, UpdateSaleResult>
    {
        private readonly ISaleRepository _repository;
        private readonly IMapper _mapper;
        private readonly ILogger<UpdateSaleHandler> _logger;

        public UpdateSaleHandler(ISaleRepository repository, IMapper mapper, ILogger<UpdateSaleHandler> logger)
        {
            _repository = repository;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<UpdateSaleResult> Handle(UpdateSaleCommand request, CancellationToken cancellationToken)
        {
            var sale = await _repository.GetByIdAsync(request.Id, cancellationToken);
            if (sale == null)
                throw new InvalidOperationException($"Sale with Id '{request.Id}' not found");

            // Atualizar dados principais
            sale.Update(request.Number, request.Date, request.Customer, request.Branch);

            // Substituir itens
            sale.ClearItems();
            foreach (var item in request.Items)
            {
                sale.AddItem(item.Sku, item.Name, item.Quantity, item.UnitPrice, item.Discount);
            }

            await _repository.UpdateAsync(sale, cancellationToken);

            // Evento (diferencial): SaleModified
            _logger.LogInformation(
                "DomainEvent=SaleModified SaleId={SaleId} Number={Number} Items={Items} TotalAmount={TotalAmount}",
                sale.Id,
                sale.Number,
                sale.Items.Select(i => new
                {
                    i.Sku,
                    i.Name,
                    i.Quantity,
                    i.UnitPrice,
                    i.Discount,
                    i.Total
                }),
                sale.TotalAmount
            );

            return _mapper.Map<UpdateSaleResult>(sale);
        }
    }
}
