// IT21217586
// Sevindi P.V.D

using EcomWave.Models;
using EcomWave.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using System.Threading.Tasks;

namespace EcomWave.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductController : ControllerBase
    {
        private readonly ProductService _productService;
        private readonly InventoryService _inventoryService;

        public ProductController(ProductService productService, InventoryService inventoryService)
        {
            _productService = productService;
            _inventoryService = inventoryService; 
        }

        // GET: api/product
        [HttpGet]
        public async Task<IActionResult> GetAllProducts()
        {
            var products = await _productService.GetAllProductsAsync();
            return Ok(products);
        }

        // GET: api/product/{id}
        [HttpGet("{productId}")]
        public async Task<IActionResult> GetProductById(string productId)
        {
            var product = await _productService.GetProductByIdAsync(productId);
            if (product == null)
                return NotFound();

            return Ok(product);
        }

        [HttpPost]
        [Authorize(Roles = "Vendor,Admin")]
        public async Task<IActionResult> CreateProduct([FromBody] Product product)
        {
            var vendorId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (string.IsNullOrEmpty(vendorId))
            {
                return BadRequest(new { message = "Vendor ID is missing or invalid in the token." });
            }

            product.IsActive = true;
            product.VendorId = vendorId;
            product.CreatedDate = DateTime.UtcNow;

            var productQuantity = product.Quantity;
            product.Quantity = null;

            await _productService.CreateProductAsync(product);

            var inventory = new Inventory
            {
                ProductId = product.ProductId,
                Quantity = (int)productQuantity,
                LowStockThreshold = 10
            };

            await _inventoryService.CreateInventoryAsync(inventory);

            return Ok(new { message = "Product and inventory created successfully.", product });
        }

        [HttpPut("{productId}")]
        [Authorize(Roles = "Vendor,Admin")]
        public async Task<IActionResult> UpdateProduct(string productId, [FromBody] Product updatedProduct, int newQuantity)
        {
            try
            {
                // Update the product
                await _productService.UpdateProductAsync(productId, updatedProduct);

                // Update the inventory quantity
                await _inventoryService.UpdateInventoryQuantityAsync(productId, newQuantity);

                return Ok(new { message = "Product and inventory updated successfully." });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while updating the product.", error = ex.Message });
            }
        }



        // DELETE: api/product/{id} (Vendor can delete)
        [HttpDelete("{productId}")]
        [Authorize(Roles = "Vendor")]
        public async Task<IActionResult> DeleteProduct(string productId)
        {
            try
            {
                await _productService.DeleteProductAsync(productId);
                return Ok(new { message = "Product deleted successfully." });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while deleting the product.", error = ex.Message });
            }
        }


        // PATCH: api/product/{id}/status (Only Admin can activate/deactivate product)
        [HttpPatch("{productId}/status")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> SetProductStatus(string productId, [FromQuery] bool isActive)
        {
            try
            {
                await _productService.SetProductStatusAsync(productId, isActive);
                return Ok(new { message = $"Product status set to {(isActive ? "active" : "inactive")} successfully." });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while setting the product status.", error = ex.Message });
            }
        }

        // GET: api/product/status?isActive=true
        [HttpGet("status")]
        public async Task<IActionResult> GetProductsByStatus([FromQuery] bool isActive)
        {
            var products = await _productService.GetProductsByStatusAsync(isActive);
            return Ok(products);
        }


    }
}
