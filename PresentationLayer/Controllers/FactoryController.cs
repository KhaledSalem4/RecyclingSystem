using BusinessLogicLayer.DTOs;
using BusinessLogicLayer.IServices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace RecyclingSystem.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FactoryController : ControllerBase
    {
        private readonly IFactoryService _factoryService;

        public FactoryController(IFactoryService factoryService)
        {
            _factoryService = factoryService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var factories = await _factoryService.GetAllFactoriesAsync();
            return Ok(factories);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var factory = await _factoryService.GetFactoryByIdAsync(id);
            if (factory == null)
                return NotFound();

            return Ok(factory);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateFactoryDto dto)
        {
            var factory = await _factoryService.CreateFactoryAsync(dto);
            return CreatedAtAction(nameof(GetById), new { id = factory.ID }, factory);
        }

        [HttpPut]
        public async Task<IActionResult> Update([FromBody] UpdateFactoryDto dto)
        {
            try
            {
                var factory = await _factoryService.UpdateFactoryAsync(dto);
                return Ok(factory);
            }
            catch (KeyNotFoundException)
            {
                return NotFound();
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                var result = await _factoryService.DeleteFactoryAsync(id);
                if (!result)
                    return NotFound();

                return NoContent();
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("{id}/details")]
        public async Task<IActionResult> GetWithOrders(int id)
        {
            var factory = await _factoryService.GetFactoryWithOrdersAsync(id);
            if (factory == null)
                return NotFound();

            return Ok(factory);
        }
    }
}
