using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using OrderApi.Controllers;
using OrderApi.Data;
using OrderApi.Models;
using OrderApi.Models.Dto;
using Microsoft.AspNetCore.Mvc;
using Xunit;


namespace OrderApi.Tests;

public class OrderControllerTests : IDisposable
{
    private readonly OrderContext _context;
    private readonly OrderController _controller;

    public OrderControllerTests()
    {
        var options = new DbContextOptionsBuilder<OrderContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        _context = new OrderContext(options);
        _context.Database.EnsureCreated();
        
        _context.Customers.Add(new Customer
        {
            Id = 100,
            Name = "Test Customer",
            Email = "test@example.com"
        });
        _context.SaveChanges();

        _controller = new OrderController(_context);
    }

    public void Dispose()
    {
        _context.Dispose();
    }

    [Fact]
    public async Task GetOrders_ReturnsOrdersList()
    {
        var result = await _controller.GetOrders();

        var okResult = result.Result.Should().BeOfType<OkObjectResult>().Subject;
        var orders = okResult.Value.Should().BeAssignableTo<IEnumerable<OrderSummaryResponse>>().Subject;
        orders.Should().NotBeEmpty();
    }

    [Fact]
    public async Task CreateOrder_WithValidData_ReturnsCreatedOrder()
    {
        var request = new CreateOrderRequest(
            CustomerId: 100,
            Items: new List<CreateOrderItemRequest>
            {
                new("Test Product", "SKU-001", 2, 25.99m)
            }
        );

        var result = await _controller.CreateOrder(request);

        var createdResult = result.Result.Should().BeOfType<CreatedAtActionResult>().Subject;
        var order = createdResult.Value.Should().BeOfType<OrderResponse>().Subject;
        
        order.CustomerId.Should().Be(100);
        order.Items.Should().HaveCount(1);
        order.TotalAmount.Should().Be(51.98m);
        order.OrderNumber.Should().StartWith("ORD-");
    }

    [Fact]
    public async Task CreateOrder_ConcurrentRequests_ShouldNotCreateDuplicateOrderNumbers()
    {
        var request = new CreateOrderRequest(
            CustomerId: 100,
            Items: new List<CreateOrderItemRequest>
            {
                new("Concurrent Product", "SKU-RACE", 1, 10.00m)
            }
        );

        var tasks = Enumerable.Range(0, 10)
            .Select(_ => CreateOrderInNewScope(request))
            .ToArray();

        var results = await Task.WhenAll(tasks);

        var successfulOrders = results
            .Where(r => r.Result is CreatedAtActionResult)
            .Select(r => ((CreatedAtActionResult)r.Result!).Value as OrderResponse)
            .Where(o => o != null)
            .ToList();

        var orderNumbers = successfulOrders.Select(o => o!.OrderNumber).ToList();
        
        orderNumbers.Should().OnlyHaveUniqueItems(
            "concurrent order creation should not produce duplicate order numbers");
    }

    private async Task<ActionResult<OrderResponse>> CreateOrderInNewScope(CreateOrderRequest request)
    {
        var options = new DbContextOptionsBuilder<OrderContext>()
            .UseSqlServer(_context.Database.GetConnectionString() ?? 
                "Server=localhost,1433;Database=OrderDb;User Id=sa;Password=YourStrong@Passw0rd;TrustServerCertificate=True;")
            .Options;

        var controller = new OrderController(_context);
        return await controller.CreateOrder(request);
    }
}
