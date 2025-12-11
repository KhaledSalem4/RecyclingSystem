using Microsoft.AspNetCore.Http;

namespace BussinessLogicLayer.DTOs.Reward
{
    public class CreateRewardWithImageDto
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public string Category { get; set; }
        public int RequiredPoints { get; set; }
        public int StockQuantity { get; set; }
        public string? ImageUrl { get; set; } // Optional: for external URLs
        public IFormFile? ImageFile { get; set; } // For file upload from device
    }
}
