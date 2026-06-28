using OrderManagementAPI.Domain.Entities;

namespace OrderManagementAPI.Application.Repositories;

public interface IOrderRepository
{
    Task AddAsync(Order order);
    Task<Order?> GetByIdAsync(Guid id);
    Task<(List<Order> Orders, int TotalCount)> GetPaginatedAsync(int page, int pageSize);
    Task UpdateAsync(Order order);
    Task SaveChangesAsync();
}