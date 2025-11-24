
namespace BusinessLogicLayer.DTOs
{
    public class OrderDto
    {
        public int ID { get; set; }
        public string Status { get; set; }
        public DateOnly OrderDate { get; set; }
        public string UserId { get; set; }
        public string? CollectorId { get; set; }
        public int FactoryId { get; set; }
    }
}
