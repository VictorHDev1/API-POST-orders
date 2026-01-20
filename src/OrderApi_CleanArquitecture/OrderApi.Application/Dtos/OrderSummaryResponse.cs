using OrderApi.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrderApi.Application.Dtos
{
    public record OrderSummaryResponse(
       int Id,
       string OrderNumber,
       //string CustomerName,
       OrderStatus Status,
       decimal TotalAmount,
       int ItemsCount,
       DateTime CreatedAt
   );
}
