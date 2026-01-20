using Microsoft.EntityFrameworkCore;
using OrderApi.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrderApi.Infrastructure.Persistence
{
    public class OrderContext : DbContext
    {
        public OrderContext(DbContextOptions<OrderContext> options)
            : base(options) { }

        public DbSet<Order> Orders => Set<Order>();
        public DbSet<OrderItem> OrderItems => Set<OrderItem>();
        public DbSet<Customer> Customers => Set<Customer>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Order>(entity =>
            {
                entity.ToTable("Orders");

                entity.HasMany(o => o.Items)
                      .WithOne(i => i.Order)
                      .HasForeignKey(i => i.OrderId)
                      .OnDelete(DeleteBehavior.Cascade);
            });

            modelBuilder.Entity<OrderItem>(entity =>
            {
                entity.ToTable("OrderItems"); // 👈 IMPORTANTE
            });

            modelBuilder.Entity<Customer>(entity =>
            {
                entity.ToTable("Customers");
            });
        }
    }
}
