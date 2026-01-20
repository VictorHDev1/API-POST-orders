using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrderApi.Application.Dtos
{
    public record CreateOrderItemRequest(
    string ProductName,
    string ProductSku,
    int Quantity,
    decimal UnitPrice
);
}
