using MediatR;
using OrderManagementAPI.Application.DTOs;

namespace OrderManagementAPI.Application.Commands;

public class CreateOrderCommand : IRequest<OrderResponseDto>
{
    public Guid CustomerId { get; set; }
    public List<OrderItemDto> Items { get; set; } = [];
}