using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayer.Entities
{
    public class Material
    {
        public int ID { get; set; }
        public string? TypeName { get; set; } = MaterialType.Plastic.ToString();
        public double Size { get; set; }
        public decimal Price { get; set; }

        public ICollection<Order> Orders { get; set; } = new HashSet<Order>();

        public int? FactoryId { get; set; }
        public Factory? Factory { get; set; }
    }
}
