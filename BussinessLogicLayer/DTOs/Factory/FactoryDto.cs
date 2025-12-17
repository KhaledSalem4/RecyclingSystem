namespace BussinessLogicLayer.DTOs.Factory
{
    public class FactoryDto
    {
        public int ID { get; set; }
        public string Name { get; set; }
        public string Location { get; set; }
        
        // Detailed Address
        public string? City { get; set; }
        public string? Street { get; set; }
        public string? BuildingNo { get; set; }
        public string? Area { get; set; }
    }
}
