using OrderApi.Application.Dtos;
using OrderApi.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrderApi.Application.Mappers
{
    public static class OrderMapper
    {
        /// <summary>
        /// Maps Order domain entity to OrderResponse DTO.
        /// </summary>
        public static OrderResponse ToResponse(Order order, string customerName)
        {
            return new OrderResponse(
                order.Id,
                order.OrderNumber,
                order.CustomerId,
                customerName,
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
        }

        /// <summary>
        /// Maps Order domain entity to OrderSummaryResponse DTO.
        /// Used for listings and pagination.
        /// </summary>
        public static OrderSummaryResponse ToSummaryResponse(Order order)
        {
            return new OrderSummaryResponse(
                order.Id,
                order.OrderNumber,
               // order.CustomerName?? string.Empty,
                order.Status,
                order.TotalAmount,
                order.Items.Count,
                order.CreatedAt
            );
        }
    }
}
