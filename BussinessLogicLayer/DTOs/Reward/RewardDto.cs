namespace BussinessLogicLayer.DTOs.Reward
{
    public class RewardDto
    {
        public int ID { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string Category { get; set; }
        public int RequiredPoints { get; set; }
        public int StockQuantity { get; set; }
        public bool IsAvailable { get; set; }
        public string ImageUrl { get; set; }


    }
}
