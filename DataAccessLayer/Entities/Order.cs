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
        public DateTime OrderDate { get; set; } = DateTime.Now; // Changed from DateOnly to DateTime

        public MaterialType TypeOfMaterial { get; set; }
        public double Quantity { get; set; }

        public OrderStatus Status { get; set; } = OrderStatus.Pending;

        public string UserId { get; set; }
        public ApplicationUser? User { get; set; }

        public string? CollectorId { get; set; }
        public ApplicationUser? Collector { get; set; }

        public int FactoryId { get; set; }
        public Factory? Factory { get; set; }

        public string? City { get; set; }
        public string? Street { get; set; }
        public string? BuildingNo { get; set; }
        public string? Apartment { get; set; }

        public ICollection<Material> Materials { get; set; }

    }

}
