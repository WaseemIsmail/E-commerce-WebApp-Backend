using EcomWave.Configurations;
using EcomWave.Models;
using MongoDB.Driver;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace EcomWave.Repositories
{
    public class OrderRepository
    {
        private readonly IMongoCollection<Order> _orders;

        public OrderRepository(MongoDbContext context)
        {
            _orders = context.Orders;
        }

        // Create a new order
        public async Task CreateOrderAsync(Order order)
        {
            await _orders.InsertOneAsync(order);
        }

        // Get order by ID
        public async Task<Order> GetOrderByIdAsync(string orderId)
        {
            return await _orders.Find(p => p.OrderId == orderId).FirstOrDefaultAsync();
        }

        // Update a order by ID
        public async Task UpdateOrderAsync(string orderId, Order updatedOrder)
        {
            var filter = Builders<Order>.Filter.Eq(p => p.OrderId, orderId);

            var update = Builders<Order>.Update
                .Set(p => p.CustomerId, updatedOrder.CustomerId)
                .Set(p => p.Items, updatedOrder.Items)
                .Set(p => p.Status, updatedOrder.Status)
                .Set(p => p.OrderDate, updatedOrder.OrderDate)
                .Set(p => p.DispatchedDate, updatedOrder.DispatchedDate)
                .Set(p => p.DeliveredDate, updatedOrder.DeliveredDate);

            await _orders.UpdateOneAsync(filter, update);
        }


        // Delete a order by ID
        public async Task DeleteOrderAsync(string orderId)
        {
            await _orders.DeleteOneAsync(p => p.OrderId == orderId);
        }

        // Get all orders
        public async Task<IEnumerable<Order>> GetAllOrdersAsync()
        {
            return await _orders.Find(_ => true).ToListAsync();
        }

        // Set order status
        public async Task SetOrderStatusAsync(string oderId, OrderStatus status)
        {
            var order = await GetOrderByIdAsync(oderId);
            if (order != null)
            {
                order.Status = status;
                await UpdateOrderAsync(oderId, order);
            }
        }

        public async Task<IEnumerable<Order>> GetOrdersByStatusAsync(OrderStatus status)
        {
            return await _orders.Find(p => p.Status == status).ToListAsync();
        }

    }
}
