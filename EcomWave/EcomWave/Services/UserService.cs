using EcomWave.Models;
using EcomWave.Repositories;
using Microsoft.IdentityModel.Tokens;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace EcomWave.Services
{
    public class UserService
    {
        private readonly UserRepository _userRepository;
        private readonly IConfiguration _configuration;

        public UserService(UserRepository userRepository, IConfiguration configuration)
        {
            _userRepository = userRepository;
            _configuration = configuration;
        }

        // Register a customer (default role is Customer)
        public async Task RegisterCustomerAsync(User user)
        {
            user.Role = UserRole.Customer;
            user.IsActive = false; // Inactive until activated by CSR/Admin
            await _userRepository.CreateUserAsync(user);
        }

        // Register a vendor (Admin only)
        public async Task RegisterVendorAsync(User user)
        {
            user.Role = UserRole.Vendor;
            user.IsActive = true; // Vendor created by Admin is active
            await _userRepository.CreateUserAsync(user);
        }

        // Register a CSR (Admin only)
        public async Task RegisterCSRAsync(User user)
        {
            user.Role = UserRole.CSR;
            user.IsActive = true; // CSR created by Admin is active
            await _userRepository.CreateUserAsync(user);
        }

        // Login method
        public async Task<string> LoginAsync(string email, string password)
        {
            var user = await _userRepository.GetUserByEmailAndPasswordAsync(email, password);

            // Check if user exists and is active
            if (user == null || !user.IsActive)
            {
                return null;
            }

            // Generate JWT token
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_configuration["Jwt:SecretKey"]);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[]
                {
                    new Claim(ClaimTypes.NameIdentifier, user.UserId),
                    new Claim(ClaimTypes.Email, user.Email),
                    new Claim(ClaimTypes.Role, user.Role.ToString())

                }),
                Expires = DateTime.UtcNow.AddHours(7),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature),
                Issuer = _configuration["Jwt:Issuer"],
                Audience = _configuration["Jwt:Audience"]
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }

        public async Task<IEnumerable<User>> GetAllUsersAsync()
        {
            return await _userRepository.GetAllUsersAsync();
        }

        public async Task<User> GetUserByIdAsync(string userId)
        {
            return await _userRepository.GetUserByIdAsync(userId);
        }

        public async Task DeactivateUserAsync(string userId)
        {
            await _userRepository.DeactivateUserAsync(userId);
        }

        public async Task ReactivateUserAsync(string userId, string role)
        {
            if (role != UserRole.CSR.ToString())
            {
                throw new UnauthorizedAccessException("Only CSR can reactivate accounts.");
            }
            await _userRepository.ReactivateUserAsync(userId);
        }
    }
}
