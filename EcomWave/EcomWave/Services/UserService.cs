using EcomWave.DTO;
using EcomWave.Models;
using EcomWave.Repositories;
using Microsoft.IdentityModel.Tokens;
using MongoDB.Driver;
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
        //public async Task<string> LoginAsync(string email, string password)
        //{
        //    var user = await _userRepository.GetUserByEmailAndPasswordAsync(email, password);

        //    // Check if user exists and is active
        //    if (user == null || !user.IsActive)
        //    {
        //        return null;
        //    }

        //    // Generate JWT token
        //    var tokenHandler = new JwtSecurityTokenHandler();
        //    var key = Encoding.ASCII.GetBytes(_configuration["Jwt:SecretKey"]);
        //    var tokenDescriptor = new SecurityTokenDescriptor
        //    {
        //        Subject = new ClaimsIdentity(new[]
        //        {
        //            new Claim(ClaimTypes.NameIdentifier, user.UserId),
        //            new Claim(ClaimTypes.Email, user.Email),
        //            new Claim(ClaimTypes.Role, user.Role.ToString())

        //        }),
        //        Expires = DateTime.UtcNow.AddHours(7),
        //        SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature),
        //        Issuer = _configuration["Jwt:Issuer"],
        //        Audience = _configuration["Jwt:Audience"]
        //    };

        //    var token = tokenHandler.CreateToken(tokenDescriptor);
        //    return tokenHandler.WriteToken(token);
        //}


        public async Task<object> LoginAsync(string email, string password)
        {
            // Retrieve user by email and password (ensure proper password hashing in production)
            var user = await _userRepository.GetUserByEmailAndPasswordAsync(email, password);

            // Check if user exists and is active
            if (user == null || user.Password != password || !user.IsActive)
            {
                return null; 
            }

            // Initialize the token handler and key
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_configuration["Jwt:SecretKey"]);

            // Generate claims based on user details
            var claims = new List<Claim>
            {
                 new Claim(ClaimTypes.NameIdentifier, user.UserId), 
                 new Claim(ClaimTypes.Email, user.Email),
                 new Claim(ClaimTypes.Role, user.Role.ToString())
             };

            // Create the token descriptor
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.UtcNow.AddHours(7), 
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature),
                Issuer = _configuration["Jwt:Issuer"],
                Audience = _configuration["Jwt:Audience"]
            };

            // Create the token
            var token = tokenHandler.CreateToken(tokenDescriptor);
            var tokenString = tokenHandler.WriteToken(token);

            // Return the serialized token as a string
            return new
            {
                email = user.Email,
                role = user.Role.ToString(),
                token = tokenString
            };
        }

        public async Task<IEnumerable<User>> GetAllUsersAsync()
        {
            return await _userRepository.GetAllUsersAsync();
        }

        public async Task<User> GetUserByIdAsync(string userId)
        {
            return await _userRepository.GetUserByIdAsync(userId);
        }

        public async Task<IEnumerable<User>> GetUsersByRoleAsync(UserRole role)
        {
            return await _userRepository.GetUsersByRoleAsync(role);
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

        // Update customer details
        public async Task<bool> UpdateCustomerAsync(string userId, CustomerUpdateDTO updateDTO)
        {
            // Retrieve the user from the repository
            var user = await _userRepository.GetUserByIdAsync(userId);

            if (user == null || user.Role != UserRole.Customer)
            {
                return false; // User not found or not a customer
            }

            // Update the user details
            user.FirstName = updateDTO.FirstName ?? user.FirstName;
            user.LastName = updateDTO.LastName ?? user.LastName;
            user.Password = updateDTO.Password ?? user.Password; 

            await _userRepository.UpdateUserAsync(user);

            return true;
        }

        // Add a rating and comment for a vendor
        public async Task AddVendorRatingAsync(string vendorId, string customerId, int rating, string comment)
        {
            var vendorRating = new VendorRating
            {
                CustomerId = customerId,
                Rating = rating,
                Comment = comment,
                RatingDate = DateTime.UtcNow
            };

            await _userRepository.AddVendorRatingAsync(vendorId, vendorRating);
            await _userRepository.UpdateAverageRatingAsync(vendorId);
        }



        // Update comment for a vendor by a specific customer
        public async Task UpdateVendorCommentAsync(string vendorId, string newComment, ClaimsPrincipal user)
        {
            var customerId = user.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (string.IsNullOrEmpty(customerId))
            {
                throw new ArgumentException("Customer ID is missing from claims.");
            }

            await _userRepository.UpdateVendorCommentAsync(vendorId, customerId, newComment);
        }





    }
}
