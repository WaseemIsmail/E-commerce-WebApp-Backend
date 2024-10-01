using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;
using System.ComponentModel.DataAnnotations;

namespace EcomWave.Models
{
    public class Vendor
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string VendorId { get; set; }

        [Required]
        [StringLength(100)]
        public string Name { get; set; }

        [StringLength(500)]
        public string Description { get; set; }

        public decimal AverageRating { get; set; }

        public List<VendorRating> Ratings { get; set; } = new List<VendorRating>();

        public DateTime CreatedDate { get; set; } = DateTime.UtcNow;

        public bool IsActive { get; set; } = true;
    }

    public class VendorRating
    {
        public string CustomerId { get; set; } 
        public int Rating { get; set; }
        public string Comment { get; set; }
        public DateTime RatingDate { get; set; } = DateTime.UtcNow;
    }
}
