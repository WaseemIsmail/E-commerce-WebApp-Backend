using EcomWave.Models;
using EcomWave.Repositories;
using EcomWave.ViewModels.Vendor;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EcomWave.Services
{
    public class VendorService
    {
        private readonly VendorRepository _vendorRepository;

        public VendorService(VendorRepository vendorRepository)
        {
            _vendorRepository = vendorRepository;
        }

        // Create a new vendor (Admin only)
        public async Task CreateVendorAsync(CreateVendorViewModel vendorViewModel)
        {
            var vendor = new Vendor
            {
                Name = vendorViewModel.Name,
                Description = vendorViewModel.Description,
                CreatedDate = DateTime.UtcNow,
                IsActive = true
            };

            await _vendorRepository.CreateVendorAsync(vendor);
        }

        // Get all vendors
        public async Task<IEnumerable<VendorViewModel>> GetAllVendorsAsync()
        {
            var vendors = await _vendorRepository.GetAllVendorsAsync();
            return vendors.Select(v => new VendorViewModel
            {
                VendorId = v.VendorId,
                Name = v.Name,
                Description = v.Description,
                AverageRating = v.AverageRating,
                IsActive = v.IsActive,
                CreatedDate = v.CreatedDate
            });
        }

        // Get vendor by ID
        public async Task<VendorViewModel> GetVendorByIdAsync(string vendorId)
        {
            var vendor = await _vendorRepository.GetVendorByIdAsync(vendorId);
            return vendor == null ? null : new VendorViewModel
            {
                VendorId = vendor.VendorId,
                Name = vendor.Name,
                Description = vendor.Description,
                AverageRating = vendor.AverageRating,
                IsActive = vendor.IsActive,
                CreatedDate = vendor.CreatedDate
            };
        }

        // Add a rating to a vendor
        public async Task AddRatingAsync(string vendorId, VendorRating rating)
        {
            await _vendorRepository.AddRatingAsync(vendorId, rating);
            await _vendorRepository.UpdateAverageRatingAsync(vendorId);
        }

        // Update a comment for a vendor rating
        public async Task UpdateCommentAsync(string vendorId, string customerId, string newComment)
        {
            await _vendorRepository.UpdateCommentAsync(vendorId, customerId, newComment);
        }

        // Activate vendor (Admin only)
        public async Task ActivateVendorAsync(string vendorId)
        {
            await _vendorRepository.SetVendorStatusAsync(vendorId, true);
        }

        // Deactivate vendor (Admin only)
        public async Task DeactivateVendorAsync(string vendorId)
        {
            await _vendorRepository.SetVendorStatusAsync(vendorId, false);
        }

        // Delete vendor (Admin only)
        public async Task DeleteVendorAsync(string vendorId)
        {
            await _vendorRepository.DeleteVendorAsync(vendorId);
        }
    }
}
