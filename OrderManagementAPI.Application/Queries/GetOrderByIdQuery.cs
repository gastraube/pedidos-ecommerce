using MediatR;
using OrderManagementAPI.Application.DTOs;

namespace OrderManagementAPI.Application.Queries;

public class GetOrderByIdQuery : IRequest<OrderResponseDto>
{
    public Guid OrderId { get; set; }
}