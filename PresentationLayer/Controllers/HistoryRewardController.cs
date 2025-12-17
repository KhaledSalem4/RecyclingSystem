using BussinessLogicLayer.DTOs.HistoryReward;
using BussinessLogicLayer.IServices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;

namespace PresentationLayer.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class HistoryRewardController : ControllerBase
    {
        private readonly IHistoryRewardService _service;

        public HistoryRewardController(IHistoryRewardService service)
        {
            _service = service;
        }

        /// <summary>
        /// Get all redemption history records (Admin only)
        /// </summary>
        [HttpGet]
        [Authorize(Policy = "AdminOnly")]
        public async Task<IActionResult> GetAll()
        {
            var items = await _service.GetAllAsync();
            return Ok(items);
        }

        /// <summary>
        /// Get paginated redemption history with filtering
        /// </summary>
        [HttpGet("paged")]
        [Authorize(Policy = "UserAccess")]
        public async Task<IActionResult> GetPaged([FromQuery] HistoryRewardQueryParams query)
        {
            var result = await _service.GetPagedAsync(query);
            return Ok(result);
        }

        /// <summary>
        /// Get specific redemption record by ID
        /// </summary>
        [HttpGet("{id:int}")]
        [Authorize(Policy = "UserAccess")]
        public async Task<IActionResult> GetById(int id)
        {
            var item = await _service.GetByIdAsync(id);
            if (item == null) return NotFound();
            return Ok(item);
        }

        /// <summary>
        /// Get all redemption history for a specific user
        /// </summary>
        [HttpGet("user/{userId}")]
        [Authorize(Policy = "UserAccess")]
        public async Task<IActionResult> GetByUser(string userId)
        {
            var items = await _service.GetByUserAsync(userId);
            return Ok(items);
        }

        /// <summary>
        /// Get redemption summary/statistics for a user
        /// </summary>
        [HttpGet("user/{userId}/summary")]
        [Authorize(Policy = "UserAccess")]
        public async Task<IActionResult> GetSummary(string userId)
        {
            var summary = await _service.GetSummaryAsync(userId);
            return Ok(summary);
        }

        /// <summary>
        /// Manually create a redemption record (Admin only)
        /// </summary>
        [HttpPost]
        [Authorize(Policy = "AdminOnly")]
        public async Task<IActionResult> Create([FromBody] CreateHistoryRewardDto dto)
        {
            if (dto == null) return BadRequest();
            var created = await _service.CreateAsync(dto);
            return CreatedAtAction(nameof(GetById), new { id = created.ID }, created);
        }

        /// <summary>
        /// Bulk create redemption records (Admin only)
        /// </summary>
        [HttpPost("bulk")]
        [Authorize(Policy = "AdminOnly")]
        public async Task<IActionResult> BulkAdd([FromBody] CreateHistoryRewardDto[] dtos)
        {
            if (dtos == null || dtos.Length == 0) return BadRequest();
            await _service.BulkAddAsync(dtos);
            return Accepted();
        }

        /// <summary>
        /// Update redemption status (Admin only) - e.g., Pending -> Completed, Cancelled
        /// </summary>
        [HttpPut("{id:int}/status")]
        [Authorize(Policy = "AdminOnly")]
        public async Task<IActionResult> UpdateStatus(int id, [FromBody] string newStatus)
        {
            if (string.IsNullOrWhiteSpace(newStatus)) return BadRequest();
            var ok = await _service.UpdateStatusAsync(id, newStatus);
            if (!ok) return NotFound();
            return NoContent();
        }

        /// <summary>
        /// Soft delete a redemption record (Admin only)
        /// </summary>
        [HttpDelete("{id:int}")]
        [Authorize(Policy = "AdminOnly")]
        public async Task<IActionResult> SoftDelete(int id)
        {
            var ok = await _service.SoftDeleteAsync(id);
            if (!ok) return NotFound();
            return NoContent();
        }
    }
}