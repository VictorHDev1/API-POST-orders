using Microsoft.EntityFrameworkCore;

namespace OrderApi.Models;

/// <summary>
/// Represents an order in the system.
/// </summary>
/// 
[Index(nameof(OrderNumber), IsUnique = true)]
public class Order
{
    public int Id { get; set; }
    
    /// <summary>
    /// Unique order number (e.g., "ORD-2024-00001").
    /// </summary>
    public string OrderNumber { get; set; } = string.Empty;
    
    public int CustomerId { get; set; }
    
    public OrderStatus Status { get; set; } = OrderStatus.Pending;
    
    public decimal TotalAmount { get; set; }
    
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    
    public DateTime? UpdatedAt { get; set; }
    
    // Navigation properties
    public Customer? Customer { get; set; }
    
    public ICollection<OrderItem> Items { get; set; } = new List<OrderItem>();
}

public enum OrderStatus
{
    Pending = 0,
    Confirmed = 1,
    Processing = 2,
    Shipped = 3,
    Delivered = 4,
    Cancelled = 5
}
