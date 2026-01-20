using OrderApi.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrderApi.Application.Interfaces
{
    public interface IOrderRepository
    {
        Task AddAsync(Order order);
        Task<Order?> GetByIdAsync(int id);
        Task<Order?> GetLastAsync();
        Task SaveChangesAsync();
    }
}
