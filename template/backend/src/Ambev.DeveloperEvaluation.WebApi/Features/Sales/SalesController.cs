using System;
using System.Threading.Tasks;
using Ambev.DeveloperEvaluation.Application.Sales.CreateSale;
using Ambev.DeveloperEvaluation.Application.Sales.GetSale;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Ambev.DeveloperEvaluation.WebApi.Features.Sales
{
    [ApiController]
    [Route("api/[controller]")]
    public class SalesController : ControllerBase
    {
        private readonly IMediator _mediator;

        public SalesController(IMediator mediator)
        {
            _mediator = mediator;
        }

        /// <summary>
        /// Create a new Sale
        /// </summary>
        [HttpPost]
        public async Task<ActionResult<CreateSaleResult>> Create([FromBody] CreateSaleCommand command)
        {
            var result = await _mediator.Send(command);
            return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
        }

        /// <summary>
        /// Get a Sale by Id
        /// </summary>
        [HttpGet("{id}")]
        public async Task<ActionResult<GetSaleResult>> GetById([FromRoute] Guid id)
        {
            var result = await _mediator.Send(new GetSaleCommand(id));
            return Ok(result);
        }
    }
}
