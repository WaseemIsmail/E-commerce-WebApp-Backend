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
            return Ok(new { message = "Customer registered successfully." });
        }


        // Login endpoint
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] UserLoginDTO loginModel)
        {
            var token = await _userService.LoginAsync(loginModel.Email, loginModel.Password);

            if (string.IsNullOrEmpty(token))
                return Unauthorized(new { message = "Invalid credentials or inactive account." });

            return Ok(new { token });
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
        public async Task<IActionResult> GetAllUsers()
        {
            var users = await _userService.GetAllUsersAsync();
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
    }
}
