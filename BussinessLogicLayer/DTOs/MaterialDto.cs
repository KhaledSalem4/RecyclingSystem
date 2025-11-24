namespace BusinessLogicLayer.DTOs
{
    public class MaterialDto
    {
        public int ID { get; set; }
        public string? TypeName { get; set; }
        public string? Size { get; set; }
        public decimal Price { get; set; }
        public int? FactoryId { get; set; }
    }
}
