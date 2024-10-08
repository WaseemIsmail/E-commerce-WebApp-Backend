using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;
using System.ComponentModel.DataAnnotations;

namespace EcomWave.Models
{
    // Inventory Model
    public class Inventory
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string InventoryId { get; set; }

        [Required]
        [BsonRepresentation(BsonType.ObjectId)]
        public string ProductId { get; set; }

        [Required]
        public int Quantity { get; set; }

        public int LowStockThreshold { get; set; }

        public DateTime LastUpdated { get; set; } = DateTime.UtcNow;
    }
}
