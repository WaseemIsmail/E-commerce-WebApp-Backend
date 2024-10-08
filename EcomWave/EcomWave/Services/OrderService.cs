using EcomWave.Models;
using EcomWave.Repositories;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace EcomWave.Services
{
    public class OrderService
    {
        private readonly OrderRepository _orderRepository;

        public OrderService(OrderRepository orderRepository)
        {
            _orderRepository = orderRepository;
        }

        public async Task CreateOrderAsync(Order order)
        {
            await _orderRepository.CreateOrderAsync(order);
        }

        public async Task<Order> GetOrderByIdAsync(string orderId)
        {
            return await _orderRepository.GetOrderByIdAsync(orderId);
        }

        public async Task UpdateOrderAsync(string orderId, Order updatedOrder)
        {
            await _orderRepository.UpdateOrderAsync(orderId, updatedOrder);
        }

        public async Task DeleteOrderAsync(string orderId)
        {
            await _orderRepository.DeleteOrderAsync(orderId);
        }

        public async Task<IEnumerable<Order>> GetAllOrdersAsync()
        {
            return await _orderRepository.GetAllOrdersAsync();
        }

        // Set order status (Admin functionality)
        public async Task SetOrderStatusAsync(string orderId, OrderStatus status)
        {
            await _orderRepository.SetOrderStatusAsync(orderId, status);
        }

        public async Task<IEnumerable<Order>> GetOrdersByStatusAsync(OrderStatus status)
        {
            return await _orderRepository.GetOrdersByStatusAsync(status);
        }

    }
}
