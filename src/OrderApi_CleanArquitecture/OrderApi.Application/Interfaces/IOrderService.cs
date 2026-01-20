using OrderApi.Application.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrderApi.Application.Interfaces
{
    public interface IOrderService
    {
        Task<OrderResponse> CreateAsync(CreateOrderRequest request);
        Task<OrderResponse?> GetByIdAsync(int id);
    }
}
