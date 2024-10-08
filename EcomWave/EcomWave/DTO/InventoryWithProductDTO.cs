// IT21277436
// Jayasinghe K.W.

using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
namespace EcomWave.DTO
{
    public class InventoryWithProductDTO
    {
        [BsonRepresentation(BsonType.ObjectId)]
        public string InventoryId { get; set; }

        [BsonRepresentation(BsonType.ObjectId)]
        public string ProductId { get; set; }

        [BsonRepresentation(BsonType.ObjectId)]
        public string _id { get; set; }
        public int Quantity { get; set; }
        public int LowStockThreshold { get; set; }
        public DateTime LastUpdated { get; set; }

        public string Name { get; set; }
        public string Description { get; set; }
        public string Price { get; set; }
        public bool IsActive { get; set; }

        [BsonRepresentation(BsonType.ObjectId)]
        public string VendorId { get; set; }
    }
}
