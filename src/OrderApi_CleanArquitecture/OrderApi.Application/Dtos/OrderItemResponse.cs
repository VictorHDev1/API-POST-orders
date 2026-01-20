using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrderApi.Application.Dtos
{
    public record OrderItemResponse(
        int Id,
        string ProductName,
        string ProductSku,
        int Quantity,
        decimal UnitPrice,
        decimal TotalPrice
    );
}
