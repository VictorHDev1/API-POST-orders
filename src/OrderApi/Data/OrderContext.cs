using Microsoft.EntityFrameworkCore;
using OrderApi.Models;

namespace OrderApi.Data;

public class OrderContext : DbContext
{
    public OrderContext(DbContextOptions<OrderContext> options) : base(options)
    {
    }

    public DbSet<Customer> Customers => Set<Customer>();
    public DbSet<Order> Orders => Set<Order>();
    public DbSet<OrderItem> OrderItems => Set<OrderItem>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Configure Customer
        modelBuilder.Entity<Customer>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Name).IsRequired().HasMaxLength(200);
            entity.Property(e => e.Email).IsRequired().HasMaxLength(200);
        });

        // Configure Order
        modelBuilder.Entity<Order>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.OrderNumber).IsRequired().HasMaxLength(50);
            entity.Property(e => e.TotalAmount).HasPrecision(18, 2);
            
            entity.HasOne(e => e.Customer)
                .WithMany(c => c.Orders)
                .HasForeignKey(e => e.CustomerId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        // Configure OrderItem
        modelBuilder.Entity<OrderItem>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.ProductName).IsRequired().HasMaxLength(200);
            entity.Property(e => e.ProductSku).IsRequired().HasMaxLength(50);
            entity.Property(e => e.UnitPrice).HasPrecision(18, 2);
            entity.Ignore(e => e.TotalPrice);
            
            entity.HasOne(e => e.Order)
                .WithMany(o => o.Items)
                .HasForeignKey(e => e.OrderId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        SeedData(modelBuilder);
    }

    private static void SeedData(ModelBuilder modelBuilder)
    {
        var customers = new List<Customer>();
        for (int i = 1; i <= 10; i++)
        {
            customers.Add(new Customer
            {
                Id = i,
                Name = $"Customer {i}",
                Email = $"customer{i}@example.com",
                CreatedAt = DateTime.UtcNow.AddDays(-100 + i)
            });
        }
        modelBuilder.Entity<Customer>().HasData(customers);

        var orders = new List<Order>();
        var orderItems = new List<OrderItem>();
        var random = new Random(42);
        int orderItemId = 1;

        for (int i = 1; i <= 100; i++)
        {
            var customerId = (i % 10) + 1;
            var itemCount = random.Next(1, 5);
            decimal total = 0;

            var order = new Order
            {
                Id = i,
                OrderNumber = $"ORD-2024-{i:D5}",
                CustomerId = customerId,
                Status = (OrderStatus)(i % 6),
                CreatedAt = DateTime.UtcNow.AddDays(-100 + i),
                TotalAmount = 0
            };

            for (int j = 1; j <= itemCount; j++)
            {
                var unitPrice = Math.Round((decimal)(random.NextDouble() * 100 + 10), 2);
                var quantity = random.Next(1, 5);
                
                orderItems.Add(new OrderItem
                {
                    Id = orderItemId++,
                    OrderId = i,
                    ProductName = $"Product {random.Next(1, 50)}",
                    ProductSku = $"SKU-{random.Next(1000, 9999)}",
                    Quantity = quantity,
                    UnitPrice = unitPrice
                });
                
                total += unitPrice * quantity;
            }

            order.TotalAmount = total;
            orders.Add(order);
        }

        modelBuilder.Entity<Order>().HasData(orders);
        modelBuilder.Entity<OrderItem>().HasData(orderItems);
    }
}
