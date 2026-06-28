using MediatR;
using OrderManagementAPI.Application.DTOs;
using OrderManagementAPI.Application.Queries;
using OrderManagementAPI.Application.Repositories;
using OrderManagementAPI.Domain.Entities;

namespace OrderManagementAPI.Application.Handlers;

public class GetOrdersQueryHandler : IRequestHandler<GetOrdersQuery, PaginatedResultDto<OrderResponseDto>>
{
    private readonly IOrderRepository _orderRepository;

    public GetOrdersQueryHandler(IOrderRepository orderRepository)
    {
        _orderRepository = orderRepository;
    }

    public async Task<PaginatedResultDto<OrderResponseDto>> Handle(GetOrdersQuery request, CancellationToken cancellationToken)
    {
        var (orders, totalCount) = await _orderRepository.GetPaginatedAsync(request.Page, request.PageSize);

        return new PaginatedResultDto<OrderResponseDto>
        {
            Data = orders.Select(MapToDto).ToList(),
            Page = request.Page,
            PageSize = request.PageSize,
            TotalCount = totalCount
        };
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