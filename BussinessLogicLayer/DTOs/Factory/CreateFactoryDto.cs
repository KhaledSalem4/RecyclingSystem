namespace BussinessLogicLayer.DTOs.Factory
{
    public class CreateFactoryDto
    {
        public string Name { get; set; } = string.Empty;
        public string Location { get; set; } = string.Empty;
        
        // Detailed Address
        public string? City { get; set; }
        public string? Street { get; set; }
        public string? BuildingNo { get; set; }
        public string? Area { get; set; }
    }
}
