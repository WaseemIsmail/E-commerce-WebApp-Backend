﻿using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;

namespace EcomWave.Models
{
    public class Order
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string OrderId { get; set; }

        public string CustomerId { get; set; } // Reference to the Customer who placed the order

        public List<OrderItem> Items { get; set; } = new List<OrderItem>();

        public OrderStatus Status { get; set; } = OrderStatus.Processing;

        public DateTime OrderDate { get; set; } = DateTime.UtcNow;

        public DateTime? DispatchedDate { get; set; }

        public DateTime? DeliveredDate { get; set; }
    }

    public class OrderItem
    {
        public string ProductId { get; set; }
        public int Quantity { get; set; }
        public decimal Price { get; set; }
    }

    public enum OrderStatus
    {
        Processing,
        Dispatched,
        Delivered,
        Cancelled
    }
}
