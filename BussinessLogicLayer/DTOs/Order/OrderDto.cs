namespace BussinessLogicLayer.DTOs.Order
{
    public class OrderDto
    {
        public int ID { get; set; }
        public string Status { get; set; } = string.Empty;
        public DateOnly OrderDate { get; set; }
        public string UserId { get; set; } = string.Empty;
        public string? CollectorId { get; set; }
        public int FactoryId { get; set; }
        
        // Navigation properties (optional, for detailed views)
        public string? UserName { get; set; }
        public string? CollectorName { get; set; }
        public string? FactoryName { get; set; }
    }
}
