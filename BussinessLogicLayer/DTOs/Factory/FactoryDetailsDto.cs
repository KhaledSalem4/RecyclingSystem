using BussinessLogicLayer.DTOs.Order;

namespace BussinessLogicLayer.DTOs.Factory
{
    public class FactoryDetailsDto
    {
        public int ID { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Location { get; set; } = string.Empty;
        public ICollection<OrderDto> Orders { get; set; } = new List<OrderDto>();
    }
}
