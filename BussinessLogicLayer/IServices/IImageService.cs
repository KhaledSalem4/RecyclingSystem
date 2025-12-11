using Microsoft.AspNetCore.Http;

namespace BusinessLogicLayer.IServices
{
    public interface IImageService
    {
        Task<string> SaveRewardImageAsync(IFormFile imageFile);
        bool DeleteRewardImage(string imageUrl);
        Task<string> UpdateRewardImageAsync(string? oldImageUrl, IFormFile newImageFile);
    }
}
