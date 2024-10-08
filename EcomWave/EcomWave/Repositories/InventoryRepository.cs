using EcomWave.DTO;
using EcomWave.Configurations;
using EcomWave.Models;
using MongoDB.Bson;
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
        public async Task<List<InventoryWithProductDTO>> GetAllInventoryLevelsAsync()
        {
            var pipeline = new[]
            {
                new BsonDocument("$lookup", new BsonDocument
                {
                    { "from", "Products" },
                    { "localField", "ProductId" },
                    { "foreignField", "_id" },
                    { "as", "productInfo" }
                }),
                new BsonDocument("$unwind", new BsonDocument("path", "$productInfo")),
                new BsonDocument("$project", new BsonDocument
                {
                    { "InventoryId", "$_id" },
                    { "ProductId", "$ProductId" },
                    { "Quantity", "$Quantity" },
                    { "LowStockThreshold", "$LowStockThreshold" },
                    { "Name", "$productInfo.Name" },
                    { "Description", "$productInfo.Description" },
                    { "Price", "$productInfo.Price" },
                    { "IsActive", "$productInfo.IsActive" },
                    { "VendorId", "$productInfo.VendorId" },
                })
            };

            return await _inventoryCollection.Aggregate<InventoryWithProductDTO>(pipeline).ToListAsync();
        }

        // Get inventory levels for a specific product
        public async Task<Inventory> GetInventoryByProductIdAsync(string productId)
        {
            return await _inventoryCollection.Find(inv => inv.ProductId == productId).FirstOrDefaultAsync();
        }

    }
}
