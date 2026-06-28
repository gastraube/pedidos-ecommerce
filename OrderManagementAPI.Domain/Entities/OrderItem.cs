namespace OrderManagementAPI.Domain.Entities;

public class OrderItem
{
    public Guid Id { get; private set; }
    public Guid OrderId { get; private set; }
    public string ProductName { get; private set; }
    public int Quantity { get; private set; }
    public decimal UnitPrice { get; private set; }

    public OrderItem(string productName, int quantity, decimal unitPrice)
    {
        if (string.IsNullOrWhiteSpace(productName))
            throw new ArgumentException("Product name is required.", nameof(productName));

        if (quantity <= 0)
            throw new ArgumentException("Quantity must be greater than zero.", nameof(quantity));

        if (unitPrice <= 0)
            throw new ArgumentException("Unit price must be greater than zero.", nameof(unitPrice));

        Id = Guid.NewGuid();
        ProductName = productName;
        Quantity = quantity;
        UnitPrice = unitPrice;
    }

    private OrderItem() { } // Para EF Core
}