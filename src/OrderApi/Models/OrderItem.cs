namespace OrderApi.Models;

/// <summary>
/// Represents a line item within an order.
/// </summary>
public class OrderItem
{
    public int Id { get; set; }
    
    public int OrderId { get; set; }
    
    public string ProductName { get; set; } = string.Empty;
    
    public string ProductSku { get; set; } = string.Empty;
    
    public int Quantity { get; set; }
    
    public decimal UnitPrice { get; set; }
    
    public decimal TotalPrice => Quantity * UnitPrice;
    
    // Navigation property
    public Order? Order { get; set; }
}
