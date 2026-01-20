using Microsoft.Extensions.Logging;
using OrderApi.Application.Dtos;
using OrderApi.Application.Interfaces;
using OrderApi.Domain.Entities;
using OrderApi.Application.Exceptions;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OrderApi.Application.Mappers;

namespace OrderApi.Application.Services
{
    public class OrderService : IOrderService
    {
        private readonly IOrderRepository _orders;
        private readonly ICustomerRepository _customers;
        private readonly ILogger<OrderService> _logger;

        public OrderService(
            IOrderRepository orders,
            ICustomerRepository customers,
            ILogger<OrderService> logger)
        {
            _orders = orders;
            _customers = customers;
            _logger = logger;
        }

        public async Task<OrderResponse> CreateAsync(CreateOrderRequest request)
        {
            _logger.LogInformation("Creating order for customer {CustomerId}", request.CustomerId);

            var customer = await _customers.GetByIdAsync(request.CustomerId)
                ?? throw new BusinessException("Customer not found");

            var last = await _orders.GetLastAsync();
            var next = (last?.Id ?? 0) + 1;

            var order = new Order
            {
                OrderNumber = $"ORD-{DateTime.UtcNow:yyyy}-{next:D5}",
                CustomerId = customer.Id
            };

            foreach (var item in request.Items)
            {
                order.Items.Add(new OrderItem
                {
                    ProductName = item.ProductName,
                    ProductSku = item.ProductSku,
                    Quantity = item.Quantity,
                    UnitPrice = item.UnitPrice
                });
            }

            order.CalculateTotal();

            await _orders.AddAsync(order);
            await _orders.SaveChangesAsync();

            return OrderMapper.ToResponse(order, customer.Name);
        }

        public async Task<OrderResponse?> GetByIdAsync(int id)
        {
            _logger.LogInformation("Getting order {OrderId}", id);

            var order = await _orders.GetByIdAsync(id);
            if (order == null)
                return null;

            var customer = await _customers.GetByIdAsync(order.CustomerId);

            return OrderMapper.ToResponse(
                order,
                customer?.Name ?? string.Empty
            );
        }

    }


}
