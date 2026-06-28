using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OrderManagementAPI.Application.Commands;
using OrderManagementAPI.Application.DTOs;
using OrderManagementAPI.Application.Queries;

namespace OrderManagementAPI.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class OrdersController : ControllerBase
{
    private readonly IMediator _mediator;

    public OrdersController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpPost]
    public async Task<ActionResult<OrderResponseDto>> CreateOrder([FromBody] CreateOrderDto request)
    {
        try
        {
            var command = new CreateOrderCommand
            {
                CustomerId = request.CustomerId,
                Items = request.Items
            };

            var result = await _mediator.Send(command);
            return CreatedAtAction(nameof(GetOrderById), new { id = result.Id }, result);
        }
        catch (Exception ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }

    [HttpGet]
    public async Task<ActionResult<PaginatedResultDto<OrderResponseDto>>> GetOrders([FromQuery] int page = 1, [FromQuery] int pageSize = 10)
    {
        var query = new GetOrdersQuery { Page = page, PageSize = pageSize };
        var result = await _mediator.Send(query);
        return Ok(result);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<OrderResponseDto>> GetOrderById(Guid id)
    {
        try
        {
            var query = new GetOrderByIdQuery { OrderId = id };
            var result = await _mediator.Send(query);
            return Ok(result);
        }
        catch (InvalidOperationException ex)
        {
            return NotFound(new { error = ex.Message });
        }
    }

    [HttpPatch("{id}/cancel")]
    public async Task<ActionResult<OrderResponseDto>> CancelOrder(Guid id)
    {
        try
        {
            var command = new CancelOrderCommand { OrderId = id };
            var result = await _mediator.Send(command);
            return Ok(result);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }
}