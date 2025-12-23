using BusinessLogicLayer.IServices;
using BussinessLogicLayer.DTOs.AppUser;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace PresentationLayer.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Policy = "AdminOnly")] // Only admins can manage collectors
    public class CollectorController : ControllerBase
    {
        private readonly IApplicationUserService _userService;

        public CollectorController(IApplicationUserService userService)
        {
            _userService = userService;
        }

        /// <summary>
        /// Hire a new collector (Admin only)
        /// </summary>
        [HttpPost("hire")]
        public async Task<IActionResult> HireCollector([FromBody] HireCollectorDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                var collector = await _userService.HireCollectorAsync(dto);
                return CreatedAtAction(nameof(GetCollectorById), new { id = collector.Id }, collector);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { error = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = "An error occurred while hiring collector", details = ex.Message });
            }
        }

        /// <summary>
        /// Get all collectors
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> GetAllCollectors()
        {
            var collectors = await _userService.GetAllCollectorsAsync();
            return Ok(collectors);
        }

        /// <summary>
        /// Get collector by ID
        /// </summary>
        [HttpGet("{id}")]
        public async Task<IActionResult> GetCollectorById(string id)
        {
            var collector = await _userService.GetCollectorByIdAsync(id);
            if (collector == null)
                return NotFound("Collector not found");

            return Ok(collector);
        }

        /// <summary>
        /// Fire a collector (permanently delete from database)
        /// </summary>
        [HttpDelete("{id}/fire")]
        public async Task<IActionResult> FireCollector(string id)
        {
            try
            {
                Console.WriteLine($"🔥 Attempting to fire and delete collector: {id}");
                
                var result = await _userService.FireCollectorAsync(id);
                
                if (!result)
                {
                    Console.WriteLine($"❌ Collector not found or not a collector: {id}");
                    return NotFound(new { error = "Collector not found or user is not a collector" });
                }

                Console.WriteLine($"✅ Collector deleted from database: {id}");
                return Ok(new 
                { 
                    success = true, 
                    message = "Collector has been permanently removed from the system" 
                });
            }
            catch (InvalidOperationException ex)
            {
                Console.WriteLine($"⚠️ Cannot fire collector: {ex.Message}");
                return BadRequest(new { error = ex.Message });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Error firing collector: {ex.Message}");
                return StatusCode(500, new 
                { 
                    error = "An error occurred while deleting the collector", 
                    details = ex.Message 
                });
            }
        }
    }
}