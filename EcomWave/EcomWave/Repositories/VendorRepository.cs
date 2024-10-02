using EcomWave.Configurations;
using EcomWave.Models;
using MongoDB.Driver;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EcomWave.Repositories
{
    public class VendorRepository
    {
        private readonly IMongoCollection<Vendor> _vendors;

        public VendorRepository(MongoDbContext context)
        {
            _vendors = context.Vendors;
        }

        // Create a new vendor
        public async Task CreateVendorAsync(Vendor vendor)
        {
            await _vendors.InsertOneAsync(vendor);
        }

        // Get all vendors
        public async Task<IEnumerable<Vendor>> GetAllVendorsAsync()
        {
            return await _vendors.Find(v => true).ToListAsync();
        }

        // Get vendor by ID
        public async Task<Vendor> GetVendorByIdAsync(string vendorId)
        {
            return await _vendors.Find(v => v.VendorId == vendorId).FirstOrDefaultAsync();
        }

        // Add a rating to a vendor
        public async Task AddRatingAsync(string vendorId, VendorRating rating)
        {
            var filter = Builders<Vendor>.Filter.Eq(v => v.VendorId, vendorId);
            var update = Builders<Vendor>.Update.Push(v => v.Ratings, rating);

            await _vendors.UpdateOneAsync(filter, update);
        }

        // Update average rating
        public async Task UpdateAverageRatingAsync(string vendorId)
        {
            var vendor = await GetVendorByIdAsync(vendorId);
            if (vendor != null && vendor.Ratings.Any())
            {
                decimal average = (decimal)vendor.Ratings.Average(r => r.Rating);
                var update = Builders<Vendor>.Update.Set(v => v.AverageRating, average);
                await _vendors.UpdateOneAsync(v => v.VendorId == vendorId, update);
            }
        }

        // Update comment for a vendor's rating
        public async Task UpdateCommentAsync(string vendorId, string customerId, string newComment)
        {
            var filter = Builders<Vendor>.Filter.And(
                Builders<Vendor>.Filter.Eq(v => v.VendorId, vendorId),
                Builders<Vendor>.Filter.ElemMatch(v => v.Ratings, r => r.CustomerId == customerId)
            );

            var update = Builders<Vendor>.Update.Set("Ratings.$.Comment", newComment);

            await _vendors.UpdateOneAsync(filter, update);
        }

        // Set vendor status (activate/deactivate)
        public async Task SetVendorStatusAsync(string vendorId, bool isActive)
        {
            var update = Builders<Vendor>.Update.Set(v => v.IsActive, isActive);
            await _vendors.UpdateOneAsync(v => v.VendorId == vendorId, update);
        }

        // Delete a vendor
        public async Task DeleteVendorAsync(string vendorId)
        {
            await _vendors.DeleteOneAsync(v => v.VendorId == vendorId);
        }
    }
}
