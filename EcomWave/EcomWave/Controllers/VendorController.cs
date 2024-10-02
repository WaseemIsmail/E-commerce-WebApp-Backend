using EcomWave.Models;
using EcomWave.Services;
using EcomWave.ViewModels.Vendor;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace EcomWave.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class VendorController : ControllerBase
    {
        private readonly VendorService _vendorService;

        public VendorController(VendorService vendorService)
        {
            _vendorService = vendorService;
        }

        // POST: api/vendors (restricted to Admins)
        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> CreateVendor([FromBody] CreateVendorViewModel vendorViewModel)
        {
            await _vendorService.CreateVendorAsync(vendorViewModel);
            return Ok(new { message = "Vendor created successfully." });
        }

        // GET: api/vendors
        [HttpGet]
        public async Task<IActionResult> GetAllVendors()
        {
            var vendors = await _vendorService.GetAllVendorsAsync();
            return Ok(vendors);
        }

        // GET: api/vendors/{id}
        [HttpGet("{id}")]
        public async Task<IActionResult> GetVendorById(string id)
        {
            var vendor = await _vendorService.GetVendorByIdAsync(id);
            return vendor == null ? NotFound() : Ok(vendor);
        }

        // POST: api/vendors/{vendorId}/ratings
        [HttpPost("{vendorId}/ratings")]
        public async Task<IActionResult> AddRating(string vendorId, [FromBody] VendorRating rating)
        {
            await _vendorService.AddRatingAsync(vendorId, rating);
            return NoContent();
        }

        // PUT: api/vendors/{vendorId}/ratings/{customerId}/comments
        [HttpPut("{vendorId}/ratings/{customerId}/comments")]
        public async Task<IActionResult> UpdateComment(string vendorId, string customerId, [FromBody] string newComment)
        {
            await _vendorService.UpdateCommentAsync(vendorId, customerId, newComment);
            return NoContent();
        }

        // PUT: api/vendors/{id}/activate (restricted to Admins)
        [HttpPut("{id}/activate")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> ActivateVendor(string id)
        {
            await _vendorService.ActivateVendorAsync(id);
            return NoContent();
        }

        // PUT: api/vendors/{id}/deactivate (restricted to Admins)
        [HttpPut("{id}/deactivate")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeactivateVendor(string id)
        {
            await _vendorService.DeactivateVendorAsync(id);
            return NoContent();
        }

        // DELETE: api/vendors/{id} (restricted to Admins)
        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteVendor(string id)
        {
            await _vendorService.DeleteVendorAsync(id);
            return NoContent();
        }
    }
}
