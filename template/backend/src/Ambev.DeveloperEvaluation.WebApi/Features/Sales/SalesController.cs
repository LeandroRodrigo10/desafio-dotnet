using System;
using System.Threading.Tasks;
using Ambev.DeveloperEvaluation.Application.Sales.CreateSale;
using Ambev.DeveloperEvaluation.Application.Sales.GetSale;
using Ambev.DeveloperEvaluation.Application.Sales.SearchSales;
using Ambev.DeveloperEvaluation.Application.Sales.UpdateSale;
using Ambev.DeveloperEvaluation.Application.Sales.DeleteSale;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;

namespace Ambev.DeveloperEvaluation.WebApi.Features.Sales
{
    [ApiController]
    [Route("api/[controller]")]
    // Protect all Sales endpoints with JWT authentication
    [Authorize]
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
            try
            {
                var result = await _mediator.Send(new GetSaleCommand(id));
                return Ok(result);
            }
            catch (InvalidOperationException ex)
            {
                return NotFound(new { message = ex.Message });
            }
        }

        /// <summary>
        /// Search sales with pagination and filters
        /// </summary>
        [HttpGet]
        public async Task<ActionResult<SearchSalesResult>> Search([FromQuery] SearchSalesCommand command)
        {
            var result = await _mediator.Send(command);
            return Ok(result);
        }

        /// <summary>
        /// Update an existing Sale
        /// </summary>
        [HttpPut("{id}")]
        public async Task<ActionResult<UpdateSaleResult>> Update([FromRoute] Guid id, [FromBody] UpdateSaleCommand command)
        {
            if (id != command.Id)
                return BadRequest("Route id and body id must match");

            var result = await _mediator.Send(command);
            return Ok(result);
        }

        /// <summary>
        /// Delete a Sale by Id
        /// </summary>
        [HttpDelete("{id}")]
        public async Task<ActionResult<DeleteSaleResult>> Delete([FromRoute] Guid id)
        {
            var result = await _mediator.Send(new DeleteSaleCommand(id));
            return Ok(result);
        }
    }
}
