using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Text.Json.Serialization;

namespace EcomWave.Models
{
    public class User
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string UserId { get; set; } = ObjectId.GenerateNewId().ToString();

        public string FirstName { get; set; }
        public string LastName { get; set; }

        public string Email { get; set; }

        public string Password { get; set; }

        [BsonRepresentation(BsonType.String)]
        public UserRole Role { get; set; } = UserRole.Customer;

        public bool IsActive { get; set; } = false; // Needs activation by CSR/Admin

        public DateTime CreatedDate { get; set; } = DateTime.UtcNow;

        public VendorDetails? VendorInfo { get; set; } = null;

    }
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public enum UserRole
    {
        Customer,
        Vendor,
        Admin,
        CSR
    }


    public class VendorDetails
    {
        public string VendorName { get; set; }
        public string Description { get; set; }
        public decimal AverageRating { get; set; }
        public List<VendorRating> Ratings { get; set; } = new List<VendorRating>();
    }

    public class VendorRating
    {
        public string CustomerId { get; set; }
        public int Rating { get; set; }
        public string Comment { get; set; }
        public DateTime RatingDate { get; set; } = DateTime.UtcNow;
    }
}
