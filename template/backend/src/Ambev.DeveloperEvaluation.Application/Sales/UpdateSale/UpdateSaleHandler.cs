using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Ambev.DeveloperEvaluation.Domain.Repositories;
using Ambev.DeveloperEvaluation.Domain.Entities;
using AutoMapper;
using MediatR;

namespace Ambev.DeveloperEvaluation.Application.Sales.UpdateSale
{
    public class UpdateSaleHandler : IRequestHandler<UpdateSaleCommand, UpdateSaleResult>
    {
        private readonly ISaleRepository _repository;
        private readonly IMapper _mapper;

        public UpdateSaleHandler(ISaleRepository repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
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

            return _mapper.Map<UpdateSaleResult>(sale);
        }
    }
}
