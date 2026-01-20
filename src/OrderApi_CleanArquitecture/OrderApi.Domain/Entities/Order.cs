using OrderApi.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrderApi.Domain.Entities
{
    public class Order
    {
        public int Id { get; set; }
        public string OrderNumber { get; set; } = string.Empty;
        public int CustomerId { get; set; }
        public OrderStatus Status { get; set; } = OrderStatus.Pending;
        public decimal TotalAmount { get; private set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }

        public ICollection<OrderItem> Items { get; set; } = new List<OrderItem>();

        public void CalculateTotal()
        {
            TotalAmount = Items.Sum(i => i.TotalPrice);
        }
    }
}
