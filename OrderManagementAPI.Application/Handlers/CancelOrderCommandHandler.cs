using MediatR;
using OrderManagementAPI.Application.Commands;
using OrderManagementAPI.Application.DTOs;

using OrderManagementAPI.Domain.Entities;
using OrderManagementAPI.Application.Repositories;

namespace OrderManagementAPI.Application.Handlers;

public class CancelOrderCommandHandler : IRequestHandler<CancelOrderCommand, OrderResponseDto>
{
    private readonly IOrderRepository _orderRepository;

    public CancelOrderCommandHandler(IOrderRepository orderRepository)
    {
        _orderRepository = orderRepository;
    }

    public async Task<OrderResponseDto> Handle(CancelOrderCommand request, CancellationToken cancellationToken)
    {
        var order = await _orderRepository.GetByIdAsync(request.OrderId);

        if (order == null)
            throw new InvalidOperationException($"Order with id {request.OrderId} not found.");

        order.Cancel();

        await _orderRepository.UpdateAsync(order);
        await _orderRepository.SaveChangesAsync();

        return MapToDto(order);
    }

    private static OrderResponseDto MapToDto(Order order)
    {
        return new OrderResponseDto
        {
            Id = order.Id,
            CustomerId = order.CustomerId,
            Status = order.Status.ToString(),
            CreatedAt = order.CreatedAt,
            TotalAmount = order.TotalAmount,
            Items = order.Items.Select(i => new OrderItemResponseDto
            {
                Id = i.Id,
                ProductName = i.ProductName,
                Quantity = i.Quantity,
                UnitPrice = i.UnitPrice
            }).ToList()
        };
    }
}