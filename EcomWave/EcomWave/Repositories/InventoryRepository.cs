using EcomWave.Configurations;
using EcomWave.Models;
using MongoDB.Driver;
using System.Threading.Tasks;

namespace EcomWave.Repositories
{
    public class InventoryRepository
    {
        private readonly IMongoCollection<Inventory> _inventoryCollection;

        public InventoryRepository(MongoDbContext context)
        {
            _inventoryCollection = context.Inventory;
        }



        // Create a new inventory entry
        public async Task CreateInventoryAsync(Inventory inventory)
        {
            await _inventoryCollection.InsertOneAsync(inventory);
        }

        // Update inventory by Product ID
        public async Task UpdateInventoryQuantityAsync(string productId, int quantity)
        {
            var filter = Builders<Inventory>.Filter.Eq(inv => inv.ProductId, productId);
            var update = Builders<Inventory>.Update
                .Set(inv => inv.Quantity, quantity)
                .Set(inv => inv.LastUpdated, DateTime.UtcNow);

            await _inventoryCollection.UpdateOneAsync(filter, update);
        }

        // Get all inventory levels
        public async Task<List<Inventory>> GetAllInventoryLevelsAsync()
        {
            return await _inventoryCollection.Find(_ => true).ToListAsync();
        }

        // Get inventory levels for a specific product
        public async Task<Inventory> GetInventoryByProductIdAsync(string productId)
        {
            return await _inventoryCollection.Find(inv => inv.ProductId == productId).FirstOrDefaultAsync();
        }

        // Get inventory levels for all products of a specific vendor
        public async Task<List<Inventory>> GetInventoryByVendorIdAsync(string vendorId)
        {
            return await _inventoryCollection.Find(inv => inv.VendorId == vendorId).ToListAsync();
        }


    }
}
