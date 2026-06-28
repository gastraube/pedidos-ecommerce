using OrderManagementAPI.Domain.Enums;

namespace OrderManagementAPI.Domain.Entities;

public class Order
{
    public Guid Id { get; private set; }
    public Guid CustomerId { get; private set; }
    public OrderStatus Status { get; private set; }
    public DateTime CreatedAt { get; private set; }
    private List<OrderItem> _items = [];

    public IReadOnlyList<OrderItem> Items => _items.AsReadOnly();

    public decimal TotalAmount => _items.Sum(x => x.UnitPrice * x.Quantity);

    public Order(Guid customerId, List<OrderItem> items)
    {
        if (items == null || items.Count == 0)
            throw new InvalidOperationException("Order must have at least one item.");

        Id = Guid.NewGuid();
        CustomerId = customerId;
        Status = OrderStatus.Pending;
        CreatedAt = DateTime.UtcNow;
        _items = items;
    }

    private Order() { } // Para EF Core

    public void Cancel()
    {
        if (Status != OrderStatus.Pending)
            throw new InvalidOperationException("Only pending orders can be cancelled.");

        Status = OrderStatus.Cancelled;
    }

    public void Confirm()
    {
        if (Status != OrderStatus.Pending)
            throw new InvalidOperationException("Only pending orders can be confirmed.");

        Status = OrderStatus.Confirmed;
    }
}