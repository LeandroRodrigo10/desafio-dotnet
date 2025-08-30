using System.Threading.Tasks;
using Ambev.DeveloperEvaluation.Application.Sales.CreateSale;
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
        /// Placeholder GET by Id (vamos implementar depois)
        /// </summary>
        [HttpGet("{id}")]
        public async Task<ActionResult<CreateSaleResult>> GetById([FromRoute] Guid id)
        {
            // Só um placeholder por enquanto
            return Ok(new { Message = "To be implemented" });
        }
    }
}
