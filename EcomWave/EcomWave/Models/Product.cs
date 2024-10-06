using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;
using System.ComponentModel.DataAnnotations;

namespace EcomWave.Models
{
    public class Product
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string ProductId { get; set; } = ObjectId.GenerateNewId().ToString();

        [Required]
        [StringLength(100)]
        public string Name { get; set; }

        [StringLength(500)]
        public string Description { get; set; }

        [Required]
        public decimal Price { get; set; }
        [Required]
        public int Quantity { get; set; }

        [BsonRepresentation(BsonType.ObjectId)]
        public string? VendorId { get; set; } // Reference to the Vendor who owns this product

        public bool IsActive { get; set; } = true; // For activation/deactivation

        public DateTime CreatedDate { get; set; } = DateTime.UtcNow;

        public DateTime? UpdatedDate { get; set; }
    }
}
