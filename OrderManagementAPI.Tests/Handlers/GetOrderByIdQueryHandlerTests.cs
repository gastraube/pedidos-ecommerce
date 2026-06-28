using FluentAssertions;
using Moq;
using OrderManagementAPI.Application.Handlers;
using OrderManagementAPI.Application.Queries;
using OrderManagementAPI.Application.Repositories;
using OrderManagementAPI.Domain.Entities;
using Xunit;

namespace OrderManagementAPI.Tests.Handlers;

public class GetOrderByIdQueryHandlerTests
{
    private readonly Mock<IOrderRepository> _repositoryMock;
    private readonly GetOrderByIdQueryHandler _handler;

    public GetOrderByIdQueryHandlerTests()
    {
        _repositoryMock = new Mock<IOrderRepository>();
        _handler = new GetOrderByIdQueryHandler(_repositoryMock.Object);
    }

    [Fact]
    public async Task Handle_WithValidOrderId_ShouldReturnOrderDto()
    {
        // Arrange
        var orderId = Guid.NewGuid();
        var customerId = Guid.NewGuid();

        var items = new List<OrderItem>
        {
            new OrderItem("Notebook", 1, 3000)
        };

        var order = new Order(customerId, items);

        _repositoryMock.Setup(r => r.GetByIdAsync(orderId)).ReturnsAsync(order);

        var query = new GetOrderByIdQuery { OrderId = orderId };

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.CustomerId.Should().Be(customerId);
        result.Status.Should().Be("Pending");
        result.TotalAmount.Should().Be(3000);
    }

    [Fact]
    public async Task Handle_WithNonExistentOrderId_ShouldThrowInvalidOperationException()
    {
        // Arrange
        var orderId = Guid.NewGuid();
        _repositoryMock.Setup(r => r.GetByIdAsync(orderId)).ReturnsAsync((Order?)null);

        var query = new GetOrderByIdQuery { OrderId = orderId };

        // Act & Assert
        await Assert.ThrowsAsync<InvalidOperationException>(() => _handler.Handle(query, CancellationToken.None));
    }
}