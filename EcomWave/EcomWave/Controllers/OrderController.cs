using EcomWave.Models;
using EcomWave.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using System.Threading.Tasks;

namespace EcomWave.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrderController : ControllerBase
    {
        private readonly OrderService _orderService;

        public OrderController(OrderService orderService)
        {
            _orderService = orderService;
        }

        // GET: api/order
        [HttpGet]
        public async Task<IActionResult> GetAllOrders()
        {
            var orders = await _orderService.GetAllOrdersAsync();
            return Ok(orders);
        }

        // GET: api/order/{id}
        [HttpGet("{orderId}")]
        public async Task<IActionResult> GetOrderById(string orderId)
        {
            var order = await _orderService.GetOrderByIdAsync(orderId);
            if (order == null)
                return NotFound();

            return Ok(order);
        }

        // POST: api/order
        [HttpPost]
        [Authorize]
        public async Task<IActionResult> CreateOrder([FromBody] Order order)
        {
            // Retrieve the customerId from the JWT token's claims
            var customerId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            // Check if customerId is retrieved properly
            if (string.IsNullOrEmpty(customerId))
            {
                return BadRequest(new { message = "Customer ID is missing or invalid in the token." });
            }

            order.CustomerId = customerId;

            await _orderService.CreateOrderAsync(order);

            return Ok(new { message = "Order created successfully.", order });
        }


        // PUT: api/order/{id} (Vendor can update)
        [HttpPut("{orderId}")]
        [Authorize(Roles = "Vendor")]
        public async Task<IActionResult> UpdateOrder(string orderId, [FromBody] Order updatedOrder)
        {
            try
            {
                await _orderService.UpdateOrderAsync(orderId, updatedOrder);
                return Ok(new { message = "Order updated successfully." });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while updating the order.", error = ex.Message });
            }
        }


        // DELETE: api/order/{id} (Vendor can delete)
        [HttpDelete("{orderId}")]
        [Authorize(Roles = "Vendor")]
        public async Task<IActionResult> DeleteOrder(string orderId)
        {
            try
            {
                await _orderService.DeleteOrderAsync(orderId);
                return Ok(new { message = "Order deleted successfully." });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while deleting the order.", error = ex.Message });
            }
        }


        // PATCH: api/order/{id}/status (Only Admin can activate/deactivate order)
        [HttpPatch("{orderId}/status")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> SetOrderStatus(string orderId, [FromQuery] OrderStatus status)
        {
            try
            {
                await _orderService.SetOrderStatusAsync(orderId, status);
                return Ok(new { message = $"Order status set to {status} successfully." });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while setting the order status.", error = ex.Message });
            }
        }

        [HttpGet("status")]
        public async Task<IActionResult> GetOrdersByStatus([FromQuery] OrderStatus status)
        {
            var orders = await _orderService.GetOrdersByStatusAsync(status);
            return Ok(orders);
        }


    }
}
