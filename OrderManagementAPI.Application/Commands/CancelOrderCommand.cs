using MediatR;
using OrderManagementAPI.Application.DTOs;

namespace OrderManagementAPI.Application.Commands;

public class CancelOrderCommand : IRequest<OrderResponseDto>
{
    public Guid OrderId { get; set; }
}