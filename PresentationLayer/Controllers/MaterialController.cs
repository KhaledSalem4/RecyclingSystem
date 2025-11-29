using BusinessLogicLayer.DTOs;
using BusinessLogicLayer.Services;
using Microsoft.AspNetCore.Mvc;

namespace PresentationLayer.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class MaterialController : ControllerBase
    {
        private readonly MaterialService _materialService;

        // Constructor Dependency Injection
        public MaterialController(MaterialService materialService)
        {
            _materialService = materialService;
        }

        // ---------------------------------------------------------
        // GET: api/material
        // Get All materials
        // ---------------------------------------------------------
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var materials = await _materialService.GetAllMaterialsAsync();
            return Ok(materials);
        }

        // ---------------------------------------------------------
        // GET: api/material/5
        // Get material by ID
        // ---------------------------------------------------------
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var material = await _materialService.GetMaterialByIdAsync(id);

            if (material == null)
                return NotFound("Material Not Found");

            return Ok(material);
        }

        // ---------------------------------------------------------
        // POST: api/material
        // Create new material
        // ---------------------------------------------------------
        [HttpPost]
        public async Task<IActionResult> Create(MaterialDto dto)
        {
            var material = await _materialService.CreateMaterialAsync(dto);
            return CreatedAtAction(nameof(GetById), new { id = material.ID }, material);
        }

        // ---------------------------------------------------------
        // PUT: api/material/5
        // Update material
        // ---------------------------------------------------------
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, MaterialDto dto)
        {
            var updated = await _materialService.UpdateMaterialAsync(id, dto);

            if (!updated)
                return NotFound("Material Not Found");

            return Ok("Updated Successfully");
        }

        // ---------------------------------------------------------
        // DELETE: api/material/5
        // Delete material
        // ---------------------------------------------------------
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var deleted = await _materialService.DeleteMaterialAsync(id);

            if (!deleted)
                return NotFound("Material Not Found");

            return Ok("Deleted Successfully");
        }

        // ---------------------------------------------------------
        // GET: api/material/factory/3
        // Get materials by factoryId
        // ---------------------------------------------------------
        [HttpGet("factory/{factoryId}")]
        public async Task<IActionResult> GetByFactory(int factoryId)
        {
            var materials = await _materialService.GetMaterialsByFactoryIdAsync(factoryId);
            return Ok(materials);
        }
    }
}
