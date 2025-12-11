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

        // Admin: get all history records
        [HttpGet]
        [Authorize(Policy = "AdminOnly")]
        public async Task<IActionResult> GetAll()
        {
            var items = await _service.GetAllAsync();
            return Ok(items);
        }

        // Paged query for UI/grid
        [HttpGet("paged")]
        [Authorize(Policy = "UserAccess")]
        public async Task<IActionResult> GetPaged([FromQuery] HistoryRewardQueryParams query)
        {
            var result = await _service.GetPagedAsync(query);
            return Ok(result);
        }

        // Get by id (user or admin)
        [HttpGet("{id:int}")]
        [Authorize(Policy = "UserAccess")]
        public async Task<IActionResult> GetById(int id)
        {
            var item = await _service.GetByIdAsync(id);
            if (item == null) return NotFound();
            return Ok(item);
        }

        // Get by user (admin or the user)
        [HttpGet("user/{userId}")]
        [Authorize(Policy = "UserAccess")]
        public async Task<IActionResult> GetByUser(string userId)
        {
            var items = await _service.GetByUserAsync(userId);
            return Ok(items);
        }

        // Create record manually (admin)
        [HttpPost]
        [Authorize(Policy = "AdminOnly")]
        public async Task<IActionResult> Create([FromBody] CreateHistoryRewardDto dto)
        {
            if (dto == null) return BadRequest();
            var created = await _service.CreateAsync(dto);
            return CreatedAtAction(nameof(GetById), new { id = created.ID }, created);
        }

        // Redeem endpoint for users
        [HttpPost("redeem")]
        [Authorize(Policy = "UserAccess")]
        public async Task<IActionResult> Redeem([FromBody] RedeemHistoryRewardDto dto)
        {
            if (dto == null) return BadRequest();
            try
            {
                var created = await _service.RedeemAsync(dto);
                return CreatedAtAction(nameof(GetById), new { id = created.ID }, created);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }

        // Update status (admin)
        [HttpPut("{id:int}/status")]
        [Authorize(Policy = "AdminOnly")]
        public async Task<IActionResult> UpdateStatus(int id, [FromBody] string newStatus)
        {
            if (string.IsNullOrWhiteSpace(newStatus)) return BadRequest();
            var ok = await _service.UpdateStatusAsync(id, newStatus);
            if (!ok) return NotFound();
            return NoContent();
        }

        // Bulk add (admin)
        [HttpPost("bulk")]
        [Authorize(Policy = "AdminOnly")]
        public async Task<IActionResult> BulkAdd([FromBody] CreateHistoryRewardDto[] dtos)
        {
            if (dtos == null || dtos.Length == 0) return BadRequest();
            await _service.BulkAddAsync(dtos);
            return Accepted();
        }

        // Soft delete (admin)
        [HttpDelete("{id:int}")]
        [Authorize(Policy = "AdminOnly")]
        public async Task<IActionResult> SoftDelete(int id)
        {
            var ok = await _service.SoftDeleteAsync(id);
            if (!ok) return NotFound();
            return NoContent();
        }

        // Summary for a user (user or admin)
        [HttpGet("{userId}/summary")]
        [Authorize(Policy = "UserAccess")]
        public async Task<IActionResult> GetSummary(string userId)
        {
            var summary = await _service.GetSummaryAsync(userId);
            return Ok(summary);
        }

        // Validate redeem (pre-check)
        [HttpPost("validate-redeem")]
        [Authorize(Policy = "UserAccess")]
        public async Task<IActionResult> ValidateRedeem([FromBody] RedeemHistoryRewardDto dto)
        {
            if (dto == null) return BadRequest();
            var ok = await _service.ValidateRedeemAsync(dto);
            return Ok(new { valid = ok });
        }
    }
}