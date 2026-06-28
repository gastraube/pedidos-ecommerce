using FluentAssertions;
using Moq;
using OrderManagementAPI.Application.Handlers;
using OrderManagementAPI.Application.Queries;
using OrderManagementAPI.Application.Repositories;
using OrderManagementAPI.Domain.Entities;
using Xunit;

namespace OrderManagementAPI.Tests.Handlers;

public class GetOrdersQueryHandlerTests
{
    private readonly Mock<IOrderRepository> _repositoryMock;
    private readonly GetOrdersQueryHandler _handler;

    public GetOrdersQueryHandlerTests()
    {
        _repositoryMock = new Mock<IOrderRepository>();
        _handler = new GetOrdersQueryHandler(_repositoryMock.Object);
    }

    [Fact]
    public async Task Handle_WithValidQuery_ShouldReturnPaginatedOrders()
    {
        // Arrange
        var customerId = Guid.NewGuid();
        var items = new List<OrderItem>
        {
            new OrderItem("Notebook", 1, 3000)
        };

        var orders = new List<Order>
        {
            new Order(customerId, items),
            new Order(customerId, items)
        };

        _repositoryMock
            .Setup(r => r.GetPaginatedAsync(1, 10))
            .ReturnsAsync((orders, 2));

        var query = new GetOrdersQuery { Page = 1, PageSize = 10 };

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Data.Should().HaveCount(2);
        result.Page.Should().Be(1);
        result.PageSize.Should().Be(10);
        result.TotalCount.Should().Be(2);
        result.TotalPages.Should().Be(1);
    }

    [Fact]
    public async Task Handle_WithEmptyResult_ShouldReturnEmptyPaginatedList()
    {
        // Arrange
        _repositoryMock
            .Setup(r => r.GetPaginatedAsync(1, 10))
            .ReturnsAsync((new List<Order>(), 0));

        var query = new GetOrdersQuery { Page = 1, PageSize = 10 };

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.Data.Should().BeEmpty();
        result.TotalCount.Should().Be(0);
    }
}