                using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayer.Entities
{
    public class Order
    {
        public int ID { get; set; }
        public DateOnly OrderDate { get; set; }

        public OrderStatus Status { get; set; } = OrderStatus.Pending;

        // User who placed the order (User role)
        public string UserId { get; set; }
        public ApplicationUser? User { get; set; }

        // Collector who handles the order (Collector role)
        public string? CollectorId { get; set; }
        public ApplicationUser? Collector { get; set; }

        // Factory where materials are delivered
        public int FactoryId { get; set; }
        public Factory? Factory { get; set; }

        public ICollection<Material> Materials { get; set; } = new HashSet<Material>();
    }
}
