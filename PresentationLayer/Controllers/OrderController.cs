using BusinessLogicLayer.IServices;
using BussinessLogicLayer.DTOs.Order;
using Microsoft.AspNetCore.Mvc;

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
        public async Task<IActionResult> Create(OrderDto dto)
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
    }
}
