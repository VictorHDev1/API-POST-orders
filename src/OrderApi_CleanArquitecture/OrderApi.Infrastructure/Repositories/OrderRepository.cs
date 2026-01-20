using Microsoft.EntityFrameworkCore;
using OrderApi.Application.Interfaces;
using OrderApi.Domain.Entities;
using OrderApi.Infrastructure.Persistence;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrderApi.Infrastructure.Repositories
{
    public class OrderRepository : IOrderRepository
    {
        private readonly OrderContext _context;

        public OrderRepository(OrderContext context)
        {
            _context = context;
        }

        public Task<Order?> GetLastAsync() =>
            _context.Orders.OrderByDescending(o => o.Id).FirstOrDefaultAsync();

        public Task<Order?> GetByIdAsync(int id) =>
            _context.Orders
                .Include(o => o.Items)
                .FirstOrDefaultAsync(o => o.Id == id);

        public async Task AddAsync(Order order) =>
            await _context.Orders.AddAsync(order);

        public Task SaveChangesAsync() =>
            _context.SaveChangesAsync();
    }
}
