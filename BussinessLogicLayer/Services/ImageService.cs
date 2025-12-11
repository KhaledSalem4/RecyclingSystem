using BusinessLogicLayer.IServices;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;

namespace BusinessLogicLayer.Services
{
    public class ImageService : IImageService
    {
        private readonly IWebHostEnvironment _environment;
        private const long MaxFileSize = 5 * 1024 * 1024; // 5MB
        private readonly string[] AllowedExtensions = { ".jpg", ".jpeg", ".png", ".gif", ".webp" };

        public ImageService(IWebHostEnvironment environment)
        {
            _environment = environment;
        }

        public async Task<string> SaveRewardImageAsync(IFormFile imageFile)
        {
            if (imageFile == null || imageFile.Length == 0)
                throw new ArgumentException("No file uploaded");

            // Validate file size
            if (imageFile.Length > MaxFileSize)
                throw new ArgumentException($"File size cannot exceed {MaxFileSize / 1024 / 1024}MB");

            // Validate file extension
            var extension = Path.GetExtension(imageFile.FileName).ToLowerInvariant();
            if (!AllowedExtensions.Contains(extension))
                throw new ArgumentException($"Only {string.Join(", ", AllowedExtensions)} files are allowed");

            // Validate content type
            if (!imageFile.ContentType.StartsWith("image/"))
                throw new ArgumentException("Only image files are allowed");

            // Create unique filename
            var fileName = $"{Guid.NewGuid()}{extension}";
            
            // Create directory if it doesn't exist
            var uploadsFolder = Path.Combine(_environment.WebRootPath, "uploads", "rewards");
            if (!Directory.Exists(uploadsFolder))
                Directory.CreateDirectory(uploadsFolder);

            // Save file
            var filePath = Path.Combine(uploadsFolder, fileName);
            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await imageFile.CopyToAsync(stream);
            }

            // Return relative URL
            return $"/uploads/rewards/{fileName}";
        }

        public bool DeleteRewardImage(string imageUrl)
        {
            if (string.IsNullOrWhiteSpace(imageUrl))
                return false;

            try
            {
                // Only delete if it's a local file (not external URL)
                if (!imageUrl.StartsWith("http"))
                {
                    // Extract filename from URL
                    var fileName = Path.GetFileName(imageUrl);
                    var filePath = Path.Combine(_environment.WebRootPath, "uploads", "rewards", fileName);

                    if (File.Exists(filePath))
                    {
                        File.Delete(filePath);
                        return true;
                    }
                }
                return false;
            }
            catch
            {
                return false;
            }
        }

        public async Task<string> UpdateRewardImageAsync(string? oldImageUrl, IFormFile newImageFile)
        {
            // Delete old image if exists
            if (!string.IsNullOrWhiteSpace(oldImageUrl))
            {
                DeleteRewardImage(oldImageUrl);
            }

            // Save new image
            return await SaveRewardImageAsync(newImageFile);
        }
    }
}
