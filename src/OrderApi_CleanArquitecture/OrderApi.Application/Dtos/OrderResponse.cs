using OrderApi.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrderApi.Application.Dtos
{
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
}
