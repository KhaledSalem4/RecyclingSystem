using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayer.Entities
{
    public class Factory
    {
        public int ID { get; set; }
        public string Name { get; set; }
        public string Location { get; set; }

        public ICollection<Order> Orders { get; set; } = new HashSet<Order>();


    }
}
