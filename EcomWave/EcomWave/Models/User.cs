using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Text.Json.Serialization;

namespace EcomWave.Models
{
    // User Model
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

        public bool IsActive { get; set; } = false;

        public DateTime CreatedDate { get; set; } = DateTime.UtcNow;

        public VendorDetails? VendorInfo { get; set; }

         public List<Notification>? Notifications { get; set; } = new List<Notification>();

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
        public decimal? AverageRating { get; set; }
        public List<VendorRating>? Ratings { get; set; } = new List<VendorRating>();
    }

    public class VendorRating
    {
        public string CustomerId { get; set; }
        public int Rating { get; set; }
        public string Comment { get; set; }
        public DateTime RatingDate { get; set; } = DateTime.UtcNow;
    }
    public class Notification
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public ObjectId NotificationId { get; set; } = ObjectId.GenerateNewId();
        public string Message { get; set; }
        public DateTime NotificationDate { get; set; } = DateTime.UtcNow;
        public bool IsRead { get; set; } = false;
    }
}
