using BusinessLogicLayer.IServices;
using BussinessLogicLayer.DTOs.Order;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace PresentationLayer.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class OrderController : ControllerBase
    {
        private readonly IOrderService _orderService;

        public OrderController(IOrderService orderService)
        {
            _orderService = orderService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var orders = await _orderService.GetAllAsync();
            return Ok(orders);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var order = await _orderService.GetByIdAsync(id);
            if (order == null)
                return NotFound("Order Not Found");
            return Ok(order);
        }

        [HttpGet("user/{userId}")]
        public async Task<IActionResult> GetByUserId(string userId)
        {
            var orders = await _orderService.GetOrdersByUserIdAsync(userId);
            return Ok(orders);
        }

        [HttpGet("collector/{collectorId}")]
        public async Task<IActionResult> GetByCollectorId(string collectorId)
        {
            var orders = await _orderService.GetOrdersByCollectorIdAsync(collectorId);
            return Ok(orders);
        }

        [HttpGet("factory/{factoryId}")]
        public async Task<IActionResult> GetByFactoryId(int factoryId)
        {
            var orders = await _orderService.GetOrdersByFactoryIdAsync(factoryId);
            return Ok(orders);
        }

        [HttpGet("status/{status}")]
        public async Task<IActionResult> GetByStatus(string status)
        {
            try
            {
                var orders = await _orderService.GetOrdersByStatusAsync(status);
                return Ok(orders);
            }
            catch (ArgumentException)
            {
                return BadRequest("Invalid status value");
            }
        }

        [HttpPost]
        public async Task<IActionResult> Create(CreateOrderDto dto)
        {
            try
            {
                var createdOrder = await _orderService.AddAsync(dto);
                return CreatedAtAction(nameof(GetById), new { id = createdOrder.ID }, createdOrder);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, OrderDto dto)
        {
            try
            {
                dto.ID = id;
                await _orderService.UpdateAsync(dto);
                return Ok("Updated Successfully");
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                await _orderService.DeleteAsync(id);
                return Ok("Deleted Successfully");
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        /// <summary>
        /// Complete an order and award points to the user based on materials recycled
        /// </summary>
        /// <param name="id">Order ID</param>
        /// <returns>Success message with total poi nts awarded</returns>
        [HttpPost("{id}/complete")]
        [Authorize(Policy = "AdminOnly")]
        public async Task<IActionResult> CompleteOrder(int id)
        {
            try
            {
                var result = await _orderService.CompleteOrderAsync(id);
                return Ok(new { success = result, message = "Order completed and points awarded" });
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { error = ex.Message });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }

        /// <summary>
        /// Cancel an order (Admin only - for any order)
        /// </summary>
        /// <param name="id">Order ID</param>
        /// <returns>Success message</returns>
        [HttpPost("{id}/cancel")]
        [Authorize(Policy = "AdminOnly")]
        public async Task<IActionResult> CancelOrder(int id)
        {
            try
            {
                var result = await _orderService.CancelOrderAsync(id);
                return Ok(new { success = result, message = "Order cancelled" });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }

        /// <summary>
        /// User cancels their own order
        /// </summary>
        /// <param name="id">Order ID</param>
        /// <returns>Success message</returns>
        [HttpPost("{id}/user-cancel")]
        [Authorize(Policy = "UserAccess")]
        public async Task<IActionResult> UserCancelOrder(int id)
        {
            try
            {
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                var result = await _orderService.UserCancelOrderAsync(id, userId);
                return Ok(new { success = result, message = "Order cancelled successfully" });
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { error = ex.Message });
            }
            catch (UnauthorizedAccessException ex)
            {
                return Forbid();
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }
    }
}
