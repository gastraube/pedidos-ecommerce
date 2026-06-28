using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc.Testing;
using OrderManagementAPI.API;
using OrderManagementAPI.Application.DTOs;
using Xunit;

namespace OrderManagementAPI.Tests.Integration;

public class OrdersControllerIntegrationTests : IAsyncLifetime
{
    private WebApplicationFactory<Program> _factory;
    private HttpClient _client;
    private string _authToken;

    public async Task InitializeAsync()
    {
        _factory = new WebApplicationFactory<Program>();
        _client = _factory.CreateClient();

        // Login first
        var loginResponse = await _client.PostAsJsonAsync("/auth/login", new
        {
            email = "dev@martech.com",
            password = "Senha@123"
        });

        loginResponse.StatusCode.Should().Be(HttpStatusCode.OK);

        var loginResult = await loginResponse.Content.ReadFromJsonAsync<JsonElement>();
        _authToken = loginResult.GetProperty("token").GetString()!;

        _client.DefaultRequestHeaders.Authorization =
            new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", _authToken);
    }

    public async Task DisposeAsync()
    {
        _client?.Dispose();
        _factory?.Dispose();
    }

    [Fact]
    public async Task CreateOrder_WithValidRequest_ShouldReturn201()
    {
        // Arrange
        var request = new
        {
            customerId = Guid.NewGuid(),
            items = new[]
            {
                new { productName = "Notebook", quantity = 1, unitPrice = 3000m }
            }
        };

        // Act
        var response = await _client.PostAsJsonAsync("/api/orders", request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Created);

        var result = await response.Content.ReadFromJsonAsync<OrderResponseDto>();
        result.Should().NotBeNull();
        result!.Status.Should().Be("Pending");
        result.TotalAmount.Should().Be(3000);
    }

    [Fact]
    public async Task GetOrders_ShouldReturnPaginatedList()
    {
        // Act
        var response = await _client.GetAsync("/api/orders?page=1&pageSize=10");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var result = await response.Content.ReadFromJsonAsync<JsonElement>();
        result.Should().NotBeNull();
    }

    [Fact]
    public async Task GetOrderById_WithValidId_ShouldReturn200()
    {
        // Arrange
        var createRequest = new
        {
            customerId = Guid.NewGuid(),
            items = new[]
            {
                new { productName = "Mouse", quantity = 2, unitPrice = 100m }
            }
        };

        var createResponse = await _client.PostAsJsonAsync("/api/orders", createRequest);
        var createdOrder = await createResponse.Content.ReadFromJsonAsync<OrderResponseDto>();

        // Act
        var response = await _client.GetAsync($"/api/orders/{createdOrder.Id}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var result = await response.Content.ReadFromJsonAsync<OrderResponseDto>();
        result!.Id.Should().Be(createdOrder!.Id);
        result.Status.Should().Be("Pending");
    }

    [Fact]
    public async Task CancelOrder_WithValidId_ShouldReturn200AndChangeStatus()
    {
        // Arrange
        var createRequest = new
        {
            customerId = Guid.NewGuid(),
            items = new[]
            {
                new { productName = "Teclado", quantity = 1, unitPrice = 500m }
            }
        };

        var createResponse = await _client.PostAsJsonAsync("/api/orders", createRequest);
        var createdOrder = await createResponse.Content.ReadFromJsonAsync<OrderResponseDto>();

        // Act
        var response = await _client.PatchAsync($"/api/orders/{createdOrder.Id}/cancel", null);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var result = await response.Content.ReadFromJsonAsync<OrderResponseDto>();
        result!.Status.Should().Be("Cancelled");
    }

    [Fact]
    public async Task CreateOrder_WithoutAuthorization_ShouldReturn401()
    {
        // Arrange
        var unauthorizedClient = _factory.CreateClient();

        var request = new
        {
            customerId = Guid.NewGuid(),
            items = new[]
            {
                new { productName = "Notebook", quantity = 1, unitPrice = 3000m }
            }
        };

        // Act
        var response = await unauthorizedClient.PostAsJsonAsync("/api/orders", request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }
}