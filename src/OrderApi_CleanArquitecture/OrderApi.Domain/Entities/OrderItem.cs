using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrderApi.Domain.Entities
{
    public class OrderItem
    {
        public int Id { get; set; }
        public int OrderId { get; set; }
        public string ProductName { get; set; } = string.Empty;
        public string ProductSku { get; set; } = string.Empty;
        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }

        public decimal TotalPrice => Quantity * UnitPrice;
        public Order Order { get; set; } = null!;
    }
}
