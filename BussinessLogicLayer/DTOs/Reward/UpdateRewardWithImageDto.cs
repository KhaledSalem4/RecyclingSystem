using Microsoft.AspNetCore.Http;

namespace BussinessLogicLayer.DTOs.Reward
{
    public class UpdateRewardWithImageDto
    {
        public int ID { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string Category { get; set; }
        public int RequiredPoints { get; set; }
        public int StockQuantity { get; set; }
        public bool IsAvailable { get; set; }
        public string? ImageUrl { get; set; } // Keep existing or use external URL
        public IFormFile? ImageFile { get; set; } // For new file upload
    }
}
