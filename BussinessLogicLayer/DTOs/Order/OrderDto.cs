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
        
        // User Address Information
        public string? UserCity { get; set; }
        public string? UserStreet { get; set; }
        public string? UserBuildingNo { get; set; }
        public string? UserApartment { get; set; }
        
        // Collector Address Information (if assigned)
        public string? CollectorCity { get; set; }
        public string? CollectorStreet { get; set; }
        public string? CollectorBuildingNo { get; set; }
        public string? CollectorApartment { get; set; }
    }
}

