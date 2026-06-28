using FluentAssertions;
using Moq;
using OrderManagementAPI.Application.Commands;
using OrderManagementAPI.Application.DTOs;
using OrderManagementAPI.Application.Handlers;
using OrderManagementAPI.Application.Repositories;
using OrderManagementAPI.Domain.Entities;
using Xunit;

namespace OrderManagementAPI.Tests.Handlers;

public class CreateOrderCommandHandlerTests
{
    private readonly Mock<IOrderRepository> _repositoryMock;
    private readonly CreateOrderCommandHandler _handler;

    public CreateOrderCommandHandlerTests()
    {
        _repositoryMock = new Mock<IOrderRepository>();
        _handler = new CreateOrderCommandHandler(_repositoryMock.Object);
    }

    [Fact]
    public async Task Handle_WithValidRequest_ShouldCreateOrderAndReturnDto()
    {
        // Arrange
        var customerId = Guid.NewGuid();
        var command = new CreateOrderCommand
        {
            CustomerId = customerId,
            Items = new List<OrderItemDto>
            {
                new OrderItemDto { ProductName = "Notebook", Quantity = 1, UnitPrice = 3000 }
            }
        };

        _repositoryMock.Setup(r => r.AddAsync(It.IsAny<Order>())).Returns(Task.CompletedTask);
        _repositoryMock.Setup(r => r.SaveChangesAsync()).Returns(Task.CompletedTask);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.CustomerId.Should().Be(customerId);
        result.Status.Should().Be("Pending");
        result.TotalAmount.Should().Be(3000);
        result.Items.Should().HaveCount(1);

        _repositoryMock.Verify(r => r.AddAsync(It.IsAny<Order>()), Times.Once);
        _repositoryMock.Verify(r => r.SaveChangesAsync(), Times.Once);
    }

    [Fact]
    public async Task Handle_WithMultipleItems_ShouldCalculateTotalAmountCorrectly()
    {
        // Arrange
        var customerId = Guid.NewGuid();
        var command = new CreateOrderCommand
        {
            CustomerId = customerId,
            Items = new List<OrderItemDto>
            {
                new OrderItemDto { ProductName = "Notebook", Quantity = 1, UnitPrice = 3000 },
                new OrderItemDto { ProductName = "Mouse", Quantity = 2, UnitPrice = 100 }
            }
        };

        _repositoryMock.Setup(r => r.AddAsync(It.IsAny<Order>())).Returns(Task.CompletedTask);
        _repositoryMock.Setup(r => r.SaveChangesAsync()).Returns(Task.CompletedTask);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.TotalAmount.Should().Be(3200); // (1 * 3000) + (2 * 100)
        result.Items.Should().HaveCount(2);
    }
}