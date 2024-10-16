﻿// IT21217586
// Sevindi P.V.D

using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;
using System.ComponentModel.DataAnnotations;

namespace EcomWave.Models
{
    // Product Model
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

        public int? Quantity { get; set; }

        [BsonRepresentation(BsonType.ObjectId)]
        public string? VendorId { get; set; } 

        public bool IsActive { get; set; } = true;

        public string? ImgUrl { get; set; }

        public DateTime CreatedDate { get; set; } = DateTime.UtcNow;

        public DateTime? UpdatedDate { get; set; }
    }
}
