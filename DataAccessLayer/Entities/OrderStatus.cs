using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayer.Entities
{
    public enum OrderStatus
    {
        Pending = 0,        // Order created, waiting for collector
        Accepted = 1,       // Collector accepted the order
        Collected = 2,     // Collector picked up materials
        Delivered = 3,      // Materials delivered to factory
        Completed = 4,      // Order completed, points awarded
        Cancelled = 5       // Order cancelled
    }
}
