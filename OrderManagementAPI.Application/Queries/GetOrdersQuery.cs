using MediatR;
using OrderManagementAPI.Application.DTOs;

namespace OrderManagementAPI.Application.Queries;

public class GetOrdersQuery : IRequest<PaginatedResultDto<OrderResponseDto>>
{
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 10;
}