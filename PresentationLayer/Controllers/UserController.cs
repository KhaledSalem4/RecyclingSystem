using BusinessLogicLayer.IServices;
using BussinessLogicLayer.DTOs.AppUser;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace PresentationLayer.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize] // All endpoints require authentication
    public class UserController : ControllerBase
    {
        private readonly IApplicationUserService _userService;

        public UserController(IApplicationUserService userService)
        {
            _userService = userService;
        }

        /// <summary>
        /// Get all users (Admin only)
        /// </summary>
        [HttpGet]
        [Authorize(Policy = "AdminOnly")]
        public async Task<IActionResult> GetAll()
        {
            var users = await _userService.GetAllAsync();
            return Ok(users);
        }

        /// <summary>
        /// Get user by ID (Admin or own profile)
        /// </summary>
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(string id)
        {
            var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var isAdmin = User.IsInRole("Admin");

            // Users can only view their own profile unless they're admin
            if (currentUserId != id && !isAdmin)
                return Forbid();

            var user = await _userService.GetByIdAsync(id);
            if (user == null)
                return NotFound("User not found");

            return Ok(user);
        }

        /// <summary>
        /// Get current user's profile
        /// </summary>
        [HttpGet("profile")]
        public async Task<IActionResult> GetMyProfile()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var profile = await _userService.GetUserProfileAsync(userId);
            
            if (profile == null)
                return NotFound("User not found");

            return Ok(profile);
        }

        /// <summary>
        /// Update current user's profile
        /// </summary>
        [HttpPut("profile")]
        public async Task<IActionResult> UpdateMyProfile([FromBody] UpdateUserDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                var result = await _userService.UpdateUserProfileAsync(userId, dto);

                if (!result)
                    return BadRequest("Failed to update profile");

                return Ok(new { success = true, message = "Profile updated successfully" });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { error = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = "An error occurred while updating profile", details = ex.Message });
            }
        }

        /// <summary>
        /// Update any user (Admin only)
        /// </summary>
        [HttpPut("{id}")]
        [Authorize(Policy = "AdminOnly")]
        public async Task<IActionResult> UpdateUser(string id, [FromBody] ApplicationUserDto dto)
        {
            if (id != dto.Id)
                return BadRequest("ID mismatch");

            try
            {
                await _userService.UpdateAsync(dto);
                return Ok(new { success = true, message = "User updated successfully" });
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = "An error occurred while updating user", details = ex.Message });
            }
        }

        /// <summary>
        /// Get current user's points
        /// </summary>
        [HttpGet("points")]
        public async Task<IActionResult> GetMyPoints()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var user = await _userService.GetByIdAsync(userId);
            
            if (user == null)
                return NotFound("User not found");

            return Ok(new { points = user.Points });
        }
    }
}
