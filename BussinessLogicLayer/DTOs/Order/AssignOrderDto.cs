using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BussinessLogicLayer.DTOs.Order
{
    public class AssignOrderDto
    {
        public int OrderId { get; set; }
        public string CollectorId { get; set; }
    }
        
    public class UpdateOrderStatusDto
    {
        public int OrderId { get; set; }
        public string NewStatus { get; set; }
    }
}
