using MediatR;
using OrderManagementAPI.Application.Commands;
using OrderManagementAPI.Application.DTOs;
using OrderManagementAPI.Application.Repositories;
using OrderManagementAPI.Domain.Entities;

namespace OrderManagementAPI.Application.Handlers;

public class CreateOrderCommandHandler : IRequestHandler<CreateOrderCommand, OrderResponseDto>
{
    private readonly IOrderRepository _orderRepository;

    public CreateOrderCommandHandler(IOrderRepository orderRepository)
    {
        _orderRepository = orderRepository;
    }

    public async Task<OrderResponseDto> Handle(CreateOrderCommand request, CancellationToken cancellationToken)
    {
        var items = request.Items
            .Select(i => new OrderItem(i.ProductName, i.Quantity, i.UnitPrice))
            .ToList();

        var order = new Order(request.CustomerId, items);

        await _orderRepository.AddAsync(order);
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