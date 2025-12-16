using BusinessLogicLayer.IServices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace PresentationLayer.Controllers
{
    [ApiController]
    [Route("api/collector/orders")]
    [Authorize(Policy = "CollectorAccess")] // Collectors and Admins can access
    public class CollectorOrderController : ControllerBase
    {
        private readonly IOrderService _orderService;

        public CollectorOrderController(IOrderService orderService)
        {
            _orderService = orderService;
        }

        /// <summary>
        /// Get all available orders (Pending, no collector assigned)
        /// </summary>
        [HttpGet("available")]
        public async Task<IActionResult> GetAvailableOrders()
        {
            var orders = await _orderService.GetAvailableOrdersForCollectorsAsync();
            return Ok(orders);
        }

        /// <summary>
        /// Get my orders as a collector
        /// </summary>
        [HttpGet("my-orders")]
        public async Task<IActionResult> GetMyOrders()
        {
            var collectorId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var orders = await _orderService.GetMyOrdersAsCollectorAsync(collectorId);
            return Ok(orders);
        }

        /// <summary>
        /// Accept/Take an available order (Collector self-assigns)
        /// </summary>
        [HttpPost("{orderId}/accept")]
        public async Task<IActionResult> AcceptOrder(int orderId)
        {
            try
            {
                var collectorId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                var result = await _orderService.CollectorAcceptOrderAsync(orderId, collectorId);

                return Ok(new
                {
                    success = result,
                    message = "Order accepted successfully",
                    orderId = orderId
                });
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
        /// Update order status (Assigned -> InProgress -> Delivered)
        /// </summary>
        [HttpPatch("{orderId}/status")]
        public async Task<IActionResult> UpdateOrderStatus(int orderId, [FromBody] UpdateOrderStatusRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.NewStatus))
                return BadRequest("Status is required");

            try
            {
                var collectorId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                var result = await _orderService.CollectorUpdateOrderStatusAsync(
                    orderId, collectorId, request.NewStatus);

                return Ok(new
                {
                    success = result,
                    message = $"Order status updated to {request.NewStatus}",
                    orderId = orderId,
                    newStatus = request.NewStatus
                });
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

    // Request models
    public class UpdateOrderStatusRequest
    {
        public string NewStatus { get; set; }
    }
}