namespace OrderApi.Models.Dto;

public record CreateOrderRequest(
    int CustomerId,
    List<CreateOrderItemRequest> Items
);

public record CreateOrderItemRequest(
    string ProductName,
    string ProductSku,
    int Quantity,
    decimal UnitPrice
);

public record UpdateOrderStatusRequest(
    OrderStatus Status
);

public record OrderResponse(
    int Id,
    string OrderNumber,
    int CustomerId,
    string CustomerName,
    OrderStatus Status,
    decimal TotalAmount,
    DateTime CreatedAt,
    List<OrderItemResponse> Items
);

public record OrderItemResponse(
    int Id,
    string ProductName,
    string ProductSku,
    int Quantity,
    decimal UnitPrice,
    decimal TotalPrice
);

public record OrderSummaryResponse(
    int Id,
    string OrderNumber,
    string CustomerName,
    OrderStatus Status,
    decimal TotalAmount,
    int ItemCount,
    DateTime CreatedAt
);

public record OrderReportResponse(
    int TotalOrders,
    decimal TotalRevenue,
    Dictionary<string, int> OrdersByStatus,
    List<CustomerOrderSummary> TopCustomers
);

public record CustomerOrderSummary(
    int CustomerId,
    string CustomerName,
    int OrderCount,
    decimal TotalSpent
);
