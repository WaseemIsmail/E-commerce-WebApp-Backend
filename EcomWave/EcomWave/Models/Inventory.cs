using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;
using System.ComponentModel.DataAnnotations;

namespace EcomWave.Models
{
    public class Inventory
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string InventoryId { get; set; }

        [Required]
        [BsonRepresentation(BsonType.ObjectId)]
        public string ProductId { get; set; } // Reference to the Product

        [Required]
        public int Quantity { get; set; }

        public int LowStockThreshold { get; set; } // Alert when quantity drops below this

        public DateTime LastUpdated { get; set; } = DateTime.UtcNow;
    }
}
