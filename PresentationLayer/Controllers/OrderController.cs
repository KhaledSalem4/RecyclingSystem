OrderController.csusing BusinessLogicLayer.DTOs;
using BusinessLogicLayer.IServices;
using Microsoft.AspNetCore.Mvc;

namespace PresentationLayer.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class OrderController : ControllerBase
    {
        private readonly IOrderService _orderService;

        // Constructor Dependency Injection
        public OrderController(IOrderService orderService)
        {
            _orderService = orderService;
        }

        // ---------------------------------------------------------
        // GET: api/order
        // Get All orders
        // ---------------------------------------------------------
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var orders = await _orderService.GetAllAsync();
            return Ok(orders);
        }

        // ---------------------------------------------------------
        // GET: api/order/5
        // Get order by ID
        // ---------------------------------------------------------
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var order = await _orderService.GetByIdAsync(id);
            if (order == null)
                return NotFound("Order Not Found");
            return Ok(order);
        }

        // ---------------------------------------------------------
        // GET: api/order/user/{userId}
        // Get orders by user ID
        // ---------------------------------------------------------
        [HttpGet("user/{userId}")]
        public async Task<IActionResult> GetByUserId(string userId)
        {
            var orders = await _orderService.GetOrdersByUserIdAsync(userId);
            return Ok(orders);
        }

        // ---------------------------------------------------------
        // GET: api/order/collector/{collectorId}
        // Get orders by collector ID
        // ---------------------------------------------------------
        [HttpGet("collector/{collectorId}")]
        public async Task<IActionResult> GetByCollectorId(string collectorId)
        {
            var orders = await _orderService.GetOrdersByCollectorIdAsync(collectorId);
            return Ok(orders);
        }

        // ---------------------------------------------------------
        // GET: api/order/factory/{factoryId}
        // Get orders by factory ID
        // ---------------------------------------------------------
        [HttpGet("factory/{factoryId}")]
        public async Task<IActionResult> GetByFactoryId(int factoryId)
        {
            var orders = await _orderService.GetOrdersByFactoryIdAsync(factoryId);
            return Ok(orders);
        }

        // ---------------------------------------------------------
        // GET: api/order/status/{status}
        // Get orders by status (Pending, InProgress, Completed, Cancelled)
        // ---------------------------------------------------------
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

        // ---------------------------------------------------------
        // POST: api/order
        // Create new order
        // ---------------------------------------------------------
        [HttpPost]
        public async Task<IActionResult> Create(CreateOrderDto dto)
        {
            try
            {
                await _orderService.AddAsync(dto);
                return CreatedAtAction(nameof(GetById), new { id = dto.ID }, dto);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        // ---------------------------------------------------------
        // PUT: api/order/{id}
        // Update order
        // ---------------------------------------------------------
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, OrderDto dto)
        {
            try
            {
                dto.ID = id; // Ensure ID matches route
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

        // ---------------------------------------------------------
        // DELETE: api/order/{id}
        // Delete order
        // ---------------------------------------------------------
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
    }
}
