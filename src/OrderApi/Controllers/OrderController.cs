using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OrderApi.Data;
using OrderApi.Models;
using OrderApi.Models.Dto;

namespace OrderApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class OrderController : ControllerBase
{
    private readonly OrderContext _context;

    public OrderController(OrderContext context)
    {
        _context = context;
    }

    /// <summary>
    /// Get all orders with pagination.
    /// </summary>
    [HttpGet]
    public async Task<ActionResult<IEnumerable<OrderSummaryResponse>>> GetOrders(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20)
    {
        var orders = await _context.Orders
            .Include(o => o.Customer)
            .OrderByDescending(o => o.CreatedAt)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .Select(o => new OrderSummaryResponse(
                o.Id,
                o.OrderNumber,
                o.Customer!.Name,
                o.Status,
                o.TotalAmount,
                o.Items.Count,
                o.CreatedAt
            ))
            .ToListAsync();

        return Ok(orders);
    }

    /// <summary>
    /// Get order by ID.
    /// </summary>
    [HttpGet("{id}")]
    public async Task<ActionResult<OrderResponse>> GetOrder(int id)
    {
        var order = await _context.Orders
            .Include(o => o.Customer)
            .Include(o => o.Items)
            .FirstOrDefaultAsync(o => o.Id == id);

        if (order == null)
            return NotFound();

        var response = new OrderResponse(
            order.Id,
            order.OrderNumber,
            order.CustomerId,
            order.Customer!.Name,
            order.Status,
            order.TotalAmount,
            order.CreatedAt,
            order.Items.Select(i => new OrderItemResponse(
                i.Id,
                i.ProductName,
                i.ProductSku,
                i.Quantity,
                i.UnitPrice,
                i.TotalPrice
            )).ToList()
        );

        return Ok(response);
    }

    /// <summary>
    /// Create a new order.
    /// </summary>
    [HttpPost]
    public async Task<ActionResult<OrderResponse>> CreateOrder([FromBody] CreateOrderRequest request)
    {
        var customer = await _context.Customers.FindAsync(request.CustomerId);
        if (customer == null)
            return BadRequest("Customer not found");

        if (request.Items == null || !request.Items.Any())
            return BadRequest("Order must have at least one item");
        
        var orderNumber  = $"ORD-{DateTime.UtcNow:yyyy}-{Guid.NewGuid().ToString("N")[..8]}";

        var order = new Order
        {
            OrderNumber = orderNumber,
            CustomerId = request.CustomerId,
            Status = OrderStatus.Pending,
            CreatedAt = DateTime.UtcNow,
            TotalAmount = 0
        };

        decimal total = 0;
        foreach (var item in request.Items)
        {
            var orderItem = new OrderItem
            {
                ProductName = item.ProductName,
                ProductSku = item.ProductSku,
                Quantity = item.Quantity,
                UnitPrice = item.UnitPrice
            };

            order.Items.Add(orderItem);
            total += item.Quantity * item.UnitPrice;
        }

        order.TotalAmount = total;

        _context.Orders.Add(order);

        try
        {
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateException)
        {
            // La DB garantiza la unicidad → esto solo ocurre en concurrencia
            return Conflict("Order number already exists");
        }

        var response = new OrderResponse(
            order.Id,
            order.OrderNumber,
            order.CustomerId,
            customer.Name,
            order.Status,
            order.TotalAmount,
            order.CreatedAt,
            order.Items.Select(i => new OrderItemResponse(
                i.Id,
                i.ProductName,
                i.ProductSku,
                i.Quantity,
                i.UnitPrice,
                i.TotalPrice
            )).ToList()
        );


        return CreatedAtAction(nameof(GetOrder), new { id = order.Id }, response);
    }

    /// <summary>
    /// Update order status.
    /// </summary>
    [HttpPatch("{id}/status")]
    public async Task<IActionResult> UpdateOrderStatus(int id, [FromBody] UpdateOrderStatusRequest request)
    {
        var order = await _context.Orders.FindAsync(id);
        if (order == null)
            return NotFound();

        order.Status = request.Status;
        order.UpdatedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync();
        return NoContent();
    }

    /// <summary>
    /// Delete an order.
    /// </summary>
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteOrder(int id)
    {
        var order = await _context.Orders.FindAsync(id);
        if (order == null)
            return NotFound();

        _context.Orders.Remove(order);
        await _context.SaveChangesAsync();

        return NoContent();
    }

    /// <summary>
    /// Get order reports.
    /// </summary>
    [HttpGet("reports")]
    public async Task<ActionResult<OrderReportResponse>> GetReports()
    {
        var totals = await _context.Orders
       .GroupBy(_ => 1)
       .Select(g => new
       {
           TotalOrders = g.Count(),
           TotalRevenue = g.Sum(o => o.TotalAmount)
       })
       .FirstOrDefaultAsync();

        var ordersByStatus = await _context.Orders
            .GroupBy(o => o.Status)
            .Select(g => new
            {
                Status = g.Key.ToString(),
                Count = g.Count()
            })
            .ToDictionaryAsync(x => x.Status, x => x.Count);

        var topCustomers = await _context.Orders
            .Join(
                _context.Customers,
                o => o.CustomerId,
                c => c.Id,
                (o, c) => new { o, c }
            )
            .GroupBy(x => new { x.c.Id, x.c.Name })
            .Select(g => new
            {
                CustomerId = g.Key.Id,
                CustomerName = g.Key.Name,
                OrderCount = g.Count(),
                TotalSpent = g.Sum(x => x.o.TotalAmount)
            })
            .OrderByDescending(x => x.TotalSpent)
            .Take(5)
            .Select(x => new CustomerOrderSummary(
                x.CustomerId,
                x.CustomerName,
                x.OrderCount,
                x.TotalSpent
            ))
            .ToListAsync();

        return Ok(new OrderReportResponse(
            totals?.TotalOrders ?? 0,
            totals?.TotalRevenue ?? 0,
            ordersByStatus,
            topCustomers
        ));
    }
}
