using FluentAssertions;
using Moq;
using OrderManagementAPI.Application.Commands;
using OrderManagementAPI.Application.Handlers;
using OrderManagementAPI.Application.Repositories;
using OrderManagementAPI.Domain.Entities;
using OrderManagementAPI.Domain.Enums;
using Xunit;

namespace OrderManagementAPI.Tests.Handlers;

public class CancelOrderCommandHandlerTests
{
    private readonly Mock<IOrderRepository> _repositoryMock;
    private readonly CancelOrderCommandHandler _handler;

    public CancelOrderCommandHandlerTests()
    {
        _repositoryMock = new Mock<IOrderRepository>();
        _handler = new CancelOrderCommandHandler(_repositoryMock.Object);
    }

    [Fact]
    public async Task Handle_WithValidPendingOrder_ShouldCancelOrder()
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
        _repositoryMock.Setup(r => r.UpdateAsync(It.IsAny<Order>())).Returns(Task.CompletedTask);
        _repositoryMock.Setup(r => r.SaveChangesAsync()).Returns(Task.CompletedTask);

        var command = new CancelOrderCommand { OrderId = orderId };

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.Status.Should().Be(OrderStatus.Cancelled.ToString());
        _repositoryMock.Verify(r => r.UpdateAsync(It.IsAny<Order>()), Times.Once);
        _repositoryMock.Verify(r => r.SaveChangesAsync(), Times.Once);
    }

    [Fact]
    public async Task Handle_WithNonExistentOrder_ShouldThrowInvalidOperationException()
    {
        // Arrange
        var orderId = Guid.NewGuid();
        _repositoryMock.Setup(r => r.GetByIdAsync(orderId)).ReturnsAsync((Order?)null);

        var command = new CancelOrderCommand { OrderId = orderId };

        // Act & Assert
        await Assert.ThrowsAsync<InvalidOperationException>(() => _handler.Handle(command, CancellationToken.None));
    }
}