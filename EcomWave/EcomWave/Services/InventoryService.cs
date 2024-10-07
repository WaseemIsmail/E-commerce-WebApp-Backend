using EcomWave.Models;
using EcomWave.Repositories;
using MongoDB.Bson;
using System.Threading.Tasks;

namespace EcomWave.Services
{
    public class InventoryService
    {
        private readonly InventoryRepository _inventoryRepository;

        public InventoryService(InventoryRepository inventoryRepository)
        {
            _inventoryRepository = inventoryRepository;
        }

        public async Task CreateInventoryAsync(Inventory inventory)
        {
            await _inventoryRepository.CreateInventoryAsync(inventory);
        }

        // Create an inventory entry
        public async Task CreateInventoryForProductAsync(string productId, int quantity, string vendorId)
        {
            var inventory = new Inventory
            {
                InventoryId = ObjectId.GenerateNewId().ToString(),
                ProductId = productId,
                Quantity = quantity,
                LowStockThreshold = 10, // or set a default value or get from vendor input
                VendorId = vendorId
            };

            await _inventoryRepository.CreateInventoryAsync(inventory);
        }

        // Update inventory quantity
        public async Task UpdateInventoryQuantityAsync(string productId, int quantity)
        {
            await _inventoryRepository.UpdateInventoryQuantityAsync(productId, quantity);
        }
    }
}
