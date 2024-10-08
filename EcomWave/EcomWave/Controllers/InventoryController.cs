// IT21277436
// Jayasinghe K.W.

using EcomWave.Models;
using EcomWave.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace EcomWave.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class InventoryController : ControllerBase
    {
        private readonly InventoryService _inventoryService;

        public InventoryController(InventoryService inventoryService)
        {
            _inventoryService = inventoryService;
        }

        // GET: api/inventory
        [HttpGet]
        public async Task<IActionResult> GetAllInventoryLevels()
        {
            try
            {
                var inventories = await _inventoryService.GetAllInventoryLevelsAsync();
                return Ok(inventories);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while fetching inventory levels.", error = ex.Message });
            }
        }

        // GET: api/inventory/product/{productId}
        [HttpGet("product/{productId}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetInventoryByProductId(string productId)
        {
            try
            {
                var inventory = await _inventoryService.GetInventoryByProductIdAsync(productId);
                if (inventory == null)
                    return NotFound(new { message = "Inventory not found for the given product ID." });

                return Ok(inventory);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while fetching inventory details.", error = ex.Message });
            }
        }
    }
}
