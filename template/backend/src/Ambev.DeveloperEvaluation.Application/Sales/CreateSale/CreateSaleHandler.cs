using System;
using System.Threading;
using System.Threading.Tasks;
using Ambev.DeveloperEvaluation.Domain.Entities;
using Ambev.DeveloperEvaluation.Domain.Repositories;
using AutoMapper;
using MediatR;

namespace Ambev.DeveloperEvaluation.Application.Sales.CreateSale
{
    // Caso seu template use MediatR (como em Auth/Users), padronizamos aqui:
    public class CreateSaleHandler : IRequestHandler<CreateSaleCommand, CreateSaleResult>
    {
        private readonly ISaleRepository _repository;
        private readonly IMapper _mapper;

        public CreateSaleHandler(ISaleRepository repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        public async Task<CreateSaleResult> Handle(CreateSaleCommand request, CancellationToken cancellationToken)
        {
            // Evita duplicidade pelo Number
            var exists = await _repository.ExistsByNumberAsync(request.Number, cancellationToken);
            if (exists)
                throw new InvalidOperationException($"A sale with number '{request.Number}' already exists.");

            // Mapear Command -> Entity (items adicionados no Profile)
            var sale = _mapper.Map<Sale>(request);

            // Persistir
            await _repository.AddAsync(sale, cancellationToken);

            // Mapear Entity -> Result
            var result = _mapper.Map<CreateSaleResult>(sale);
            return result;
        }
    }
}
