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
    public class CustomerRepository : ICustomerRepository
    {
        private readonly OrderContext _context;

        public CustomerRepository(OrderContext context)
        {
            _context = context;
        }

        public Task<Customer?> GetByIdAsync(int id) =>
            _context.Customers.FindAsync(id).AsTask();
    }
}
