// IT21215988
// Waseem M.I.M

using EcomWave.DTO;
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
    public class UserController : ControllerBase
    {
        private readonly UserService _userService;

        public UserController(UserService userService)
        {
            _userService = userService;
        }

        // Customer signup endpoint
        [HttpPost("signup")]
        public async Task<IActionResult> RegisterCustomer([FromBody] CustomerRegistrationDTO customerDTO)
        {
            var user = new User
            {
                FirstName = customerDTO.FirstName,
                LastName = customerDTO.LastName,
                Email = customerDTO.Email,
                Password = customerDTO.Password,
                Role = UserRole.Customer,
                IsActive = false,
                VendorInfo = null
            };
            
            await _userService.RegisterCustomerAsync(user);

            var notification = new Notification
            {
                Message = $"{customerDTO.FirstName} {customerDTO.LastName} has registered. Please activate their account.",
                IsRead = false
            };

            await _userService.SendNotificationByRoleAsync(notification, UserRole.CSR);


            return Ok(new { message = "Customer registered successfully." });
        }

        // Login endpoint
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] UserLoginDTO loginModel)
        {
            var data = await _userService.LoginAsync(loginModel.Email, loginModel.Password);

            if (data == null)
                return Unauthorized(new { message = "Invalid credentials or inactive account." });

            var response = data as dynamic;

            return Ok(new
            {
                response.email,
                response.role,
                response.token
            });
        }


        // Register vendor (Admin only)
        [HttpPost("vendor")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> RegisterVendor([FromBody] User vendorUser)
        {

            await _userService.RegisterVendorAsync(vendorUser);
            return Ok(new { message = "Vendor registered successfully." });
        }

        // Register CSR (Admin only)
        [HttpPost("csr")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> RegisterCSR([FromBody] User csrUser)
        {
            await _userService.RegisterCSRAsync(csrUser);
            return Ok(new { message = "CSR registered successfully." });
        }

        // Deactivate user account (User)
        [HttpPut("deactivate/{id}")]
        public async Task<IActionResult> DeactivateUser(string id)
        {
            await _userService.DeactivateUserAsync(id);
            return Ok(new { message = "User deactivated successfully." });
        }

        // Reactivate user account (CSR only)
        [HttpPut("reactivate/{id}")]
        [Authorize(Roles = "CSR")]
        public async Task<IActionResult> ReactivateUser(string id)
        {
            var userRole = User.FindFirst(claim => claim.Type == ClaimTypes.Role)?.Value;
            await _userService.ReactivateUserAsync(id, userRole);
            return Ok(new { message = "User reactivated successfully." });
        }

        // Get all users (Admin only)
        [HttpGet]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetAllUsers([FromQuery] UserRole? role)
        {
            IEnumerable<User> users;

            if (role.HasValue)
            {
                users = (IEnumerable<User>)await _userService.GetUsersByRoleAsync(role.Value);
            }
            else
            {
                users = await _userService.GetAllUsersAsync();
            }

            return Ok(users);
        }

        // Get user by ID (Admin only)
        [HttpGet("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetUserById(string id)
        {
            var user = await _userService.GetUserByIdAsync(id);
            return user == null ? NotFound() : Ok(user);
        }

        // Update user
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateUser(string id, [FromBody] CustomerUpdateDTO updateDTO)
        {
            var updated = await _userService.UpdateCustomerAsync(id, updateDTO);
            if (!updated)
            {
                return NotFound(new { message = "User not found or not a customer." });
            }

            return Ok(new { message = "User updated successfully." });
        }

        // Add rating to a vendor
        [HttpPost("{vendorId}/rating")]
        [Authorize(Roles = "Customer")]
        public async Task<IActionResult> AddVendorRating(string vendorId, [FromBody] VendorRatingDTO ratingModel)
        {
            
            var customerId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (string.IsNullOrEmpty(customerId))
            {
                return BadRequest(new { message = "Customer ID is missing or invalid in the token." });
            }

            await _userService.AddVendorRatingAsync(vendorId, customerId, ratingModel.Rating, ratingModel.Comment);

            return Ok(new { message = "Rating added successfully." });
        }



        // Update comment for a vendor by a specific customer
        [HttpPut("{vendorId}/rating/comment")]
        [Authorize(Roles = "Customer")]
        public async Task<IActionResult> UpdateVendorComment(string vendorId, [FromBody] UpdateCommentDTO commentDTO)
        {
            await _userService.UpdateVendorCommentAsync(vendorId, commentDTO.NewComment, User);
            return Ok(new { message = "Comment updated successfully." });
        }

        // Get notifications
        //[HttpGet("notification")]
        //[Authorize]
        //public async Task<IActionResult> GetUserNotifications()
        //{
        //    var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        //    var user = await _userService.GetUserByIdAsync(userId);
        //    return user == null ? NotFound() : Ok(user);
        //}

        [HttpGet("notifications")]
        [Authorize]
        public async Task<IActionResult> GetUserNotifications()
        {
            // Get the logged-in user's ID from claims
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized(new { message = "User ID is missing in the token." });
            }

            // Fetch the notifications for this user
            var notifications = await _userService.GetUserNotificationsAsync(userId);

            return Ok(notifications);
        }

    }


}
