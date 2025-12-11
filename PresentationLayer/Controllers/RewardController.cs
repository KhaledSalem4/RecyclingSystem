using BusinessLogicLayer.IServices;
using BussinessLogicLayer.DTOs.Reward;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace PresentationLayer.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RewardController : ControllerBase
    {
        private readonly IRewardService _rewardService;
        private readonly IImageService _imageService;

        public RewardController(IRewardService rewardService, IImageService imageService)
        {
            _rewardService = rewardService;
            _imageService = imageService;
        }

        // GET: api/reward
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var rewards = await _rewardService.GetAllAsync();
            return Ok(rewards);
        }

        // GET: api/reward/5
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var reward = await _rewardService.GetByIdAsync(id);
            if (reward == null)
                return NotFound();

            return Ok(reward);
        }

        // GET: api/reward/available
        [Authorize]
        [HttpGet("available")]
        public async Task<IActionResult> GetAvailableForUser()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var rewards = await _rewardService.GetAvailableRewardsForUserAsync(userId);
            return Ok(rewards);
        }

        // GET: api/reward/category/Electronics
        [HttpGet("category/{category}")]
        public async Task<IActionResult> GetByCategory(string category)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var rewards = await _rewardService.GetRewardsByCategoryAsync(category, userId);
            return Ok(rewards);
        }

        // GET: api/reward/popular
        [HttpGet("popular")]
        public async Task<IActionResult> GetPopular([FromQuery] int topCount = 10)
        {
            var rewards = await _rewardService.GetPopularRewardsAsync(topCount);
            return Ok(rewards);
        }

        // GET: api/reward/search?term=gift
        [HttpGet("search")]
        public async Task<IActionResult> Search([FromQuery] string term)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var rewards = await _rewardService.SearchRewardsAsync(term, userId);
            return Ok(rewards);
        }

        // GET: api/reward/categories
        [HttpGet("categories")]
        public async Task<IActionResult> GetCategories()
        {
            var categories = await _rewardService.GetAllCategoriesAsync();
            return Ok(categories);
        }

        // POST: api/reward
        [HttpPost]
        [Consumes("multipart/form-data")]
        public async Task<IActionResult> Create([FromForm] CreateRewardWithImageDto dto)
        {
            try
            {
                string? imageUrl = null;

                // Upload image if provided
                if (dto.ImageFile != null)
                {
                    imageUrl = await _imageService.SaveRewardImageAsync(dto.ImageFile);
                }

                var createDto = new CreateRewardDto
                {
                    Name = dto.Name,
                    Description = dto.Description,
                    Category = dto.Category,
                    RequiredPoints = dto.RequiredPoints,
                    StockQuantity = dto.StockQuantity,
                    ImageUrl = imageUrl ?? dto.ImageUrl // Use uploaded image or provided URL
                };

                var reward = await _rewardService.AddAsync(createDto);
                return CreatedAtAction(nameof(GetById), new { id = reward.ID }, reward);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { error = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = $"Internal server error: {ex.Message}" });
            }
        }

        // PUT: api/reward/5
        [HttpPut("{id}")]
        [Consumes("multipart/form-data")]
        public async Task<IActionResult> Update(int id, [FromForm] UpdateRewardWithImageDto dto)
        {
            try
            {
                if (id != dto.ID)
                    return BadRequest(new { error = "ID mismatch" });

                string? imageUrl = dto.ImageUrl;

                // Upload new image if provided
                if (dto.ImageFile != null)
                {
                    var existingReward = await _rewardService.GetByIdAsync(id);
                    imageUrl = await _imageService.UpdateRewardImageAsync(existingReward?.ImageUrl, dto.ImageFile);
                }

                var updateDto = new UpdateRewardDto
                {
                    ID = dto.ID,
                    Name = dto.Name,
                    Description = dto.Description,
                    Category = dto.Category,
                    RequiredPoints = dto.RequiredPoints,
                    StockQuantity = dto.StockQuantity,
                    IsAvailable = dto.IsAvailable,
                    ImageUrl = imageUrl
                };

                var reward = await _rewardService.UpdateAsync(updateDto);
                return Ok(reward);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { error = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = $"Internal server error: {ex.Message}" });
            }
        }

        // DELETE: api/reward/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var result = await _rewardService.DeleteAsync(id);
            return result ? NoContent() : NotFound();
        }

        // POST: api/reward/redeem
        [Authorize]
        [HttpPost("redeem")]
        public async Task<IActionResult> Redeem([FromBody] RedeemRewardDto dto)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var result = await _rewardService.RedeemRewardAsync(userId, dto);
            return Ok(new { success = result, message = "Reward redeemed successfully" });
        }

        // GET: api/reward/low-stock
        [HttpGet("low-stock")]
        public async Task<IActionResult> GetLowStock([FromQuery] int threshold = 10)
        {
            var rewards = await _rewardService.GetLowStockRewardsAsync(threshold);
            return Ok(rewards);
        }

        // GET: api/reward/5/stats
        [HttpGet("{id}/stats")]
        public async Task<IActionResult> GetStats(int id)
        {
            var stats = await _rewardService.GetRewardWithStatsAsync(id);
            if (stats == null)
                return NotFound();

            return Ok(stats);
        }

        // PATCH: api/reward/5/stock
        [HttpPatch("{id}/stock")]
        public async Task<IActionResult> UpdateStock(int id, [FromBody] int quantityChange)
        {
            var result = await _rewardService.UpdateStockAsync(id, quantityChange);
            return result ? Ok() : NotFound();
        }

    }
}
