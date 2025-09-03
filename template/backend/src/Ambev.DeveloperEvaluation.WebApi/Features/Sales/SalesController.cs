using System;
using System.Threading.Tasks;
using AutoMapper;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

using Ambev.DeveloperEvaluation.Application.Sales.SearchSales;
using Ambev.DeveloperEvaluation.Application.Sales.GetSale;
using Ambev.DeveloperEvaluation.Application.Sales.CreateSale;
using Ambev.DeveloperEvaluation.Application.Sales.UpdateSale;
using Ambev.DeveloperEvaluation.Application.Sales.DeleteSale;

using Ambev.DeveloperEvaluation.WebApi.Features.Sales.CreateSale;
using Ambev.DeveloperEvaluation.WebApi.Features.Sales.UpdateSale;
using Ambev.DeveloperEvaluation.WebApi.Features.Sales.DeleteSale;

namespace Ambev.DeveloperEvaluation.WebApi.Features.Sales
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize] // qualquer usuário autenticado pode consultar
    public class SalesController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly IMapper _mapper;

        public SalesController(IMediator mediator, IMapper mapper)
        {
            _mediator = mediator;
            _mapper = mapper;
        }

        // GET /api/Sales
        [HttpGet]
        public async Task<ActionResult<SearchSalesResult>> Search([FromQuery] SearchSalesCommand command)
        {
            var result = await _mediator.Send(command);
            return Ok(result);
        }

        // GET /api/Sales/{id}
        [HttpGet("{id}")]
        public async Task<ActionResult<GetSaleResult>> GetById(Guid id)
        {
            var result = await _mediator.Send(new GetSaleCommand(id));
            if (result == null)
                return NotFound(new { message = $"Sale with Id '{id}' not found" });

            return Ok(result);
        }

        // POST /api/Sales
        // Apenas Admin pode criar vendas
        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<CreateSaleResponse>> Create([FromBody] CreateSaleRequest request)
        {
            var command = _mapper.Map<CreateSaleCommand>(request);
            var result = await _mediator.Send(command);
            var response = _mapper.Map<CreateSaleResponse>(result);

            return CreatedAtAction(nameof(GetById), new { id = response.Id }, response);
        }

        // PUT /api/Sales/{id}
        // Apenas Admin pode atualizar vendas
        [HttpPut("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<UpdateSaleResponse>> Update([FromRoute] Guid id, [FromBody] UpdateSaleRequest request)
        {
            // Usar ctor padrão e setar o Id por propriedade
            var command = _mapper.Map<UpdateSaleCommand>(request) ?? new UpdateSaleCommand();

            command.Id = id;

            var result = await _mediator.Send(command);
            var response = _mapper.Map<UpdateSaleResponse>(result);
            return Ok(response);
        }

        // DELETE /api/Sales/{id}
        // Apenas Admin pode excluir vendas
        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<DeleteSaleResponse>> Delete([FromRoute] Guid id)
        {
            var result = await _mediator.Send(new DeleteSaleCommand(id));

            if (!result.Success)
                return NotFound(new { message = $"Sale with Id '{id}' not found" });

            var response = _mapper.Map<DeleteSaleResponse>(result);
            return Ok(response);
        }
    }
}
