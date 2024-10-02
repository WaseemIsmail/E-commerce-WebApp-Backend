using EcomWave.Models;
using EcomWave.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace EcomWave.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductController : ControllerBase
    {
        private readonly ProductService _productService;

        public ProductController(ProductService productService)
        {
            _productService = productService;
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

        // POST: api/product (Vendor can create)
        [HttpPost]
        [Authorize(Roles = "Vendor")]
        public async Task<IActionResult> CreateProduct([FromBody] Product product)
        {
            product.CreatedDate = DateTime.UtcNow;
            await _productService.CreateProductAsync(product);
            return Ok(product);
        }

        // PUT: api/product/{id} (Vendor can update)
        [HttpPut("{productId}")]
        [Authorize(Roles = "Vendor")]
        public async Task<IActionResult> UpdateProduct(string productId, [FromBody] Product updatedProduct)
        {
            await _productService.UpdateProductAsync(productId, updatedProduct);
            return NoContent();
        }

        // DELETE: api/product/{id} (Vendor can delete)
        [HttpDelete("{productId}")]
        [Authorize(Roles = "Vendor")]
        public async Task<IActionResult> DeleteProduct(string productId)
        {
            await _productService.DeleteProductAsync(productId);
            return NoContent();
        }

        // PATCH: api/product/{id}/status (Only Admin can activate/deactivate product)
        [HttpPatch("{productId}/status")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> SetProductStatus(string productId, [FromQuery] bool isActive)
        {
            await _productService.SetProductStatusAsync(productId, isActive);
            return NoContent();
        }
    }
}
