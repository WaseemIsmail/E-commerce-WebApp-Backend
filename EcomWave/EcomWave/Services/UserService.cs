using EcomWave.Models;
using EcomWave.Repositories;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace EcomWave.Services
{
    public class UserService
    {
        private readonly UserRepository _userRepository;

        public UserService(UserRepository userRepository)
        {
            _userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
        }

        // Get all users
        public Task<IEnumerable<User>> GetAllUsersAsync()
        {
            // Directly return the Task from the repository
            return _userRepository.GetAllUsersAsync();
        }

        // Get user by ID
        public async Task<User> GetUserByIdAsync(string id)
        {
            // Validate ID parameter
            if (string.IsNullOrWhiteSpace(id))
                throw new ArgumentException("Invalid user ID", nameof(id));

            var user = await _userRepository.GetUserByIdAsync(id);
            if (user == null)
                throw new KeyNotFoundException("User not found.");

            return user;
        }

        // Create a new user
        public async Task CreateUserAsync(User user)
        {
            // Additional validation logic before creating user
            if (user == null)
                throw new ArgumentNullException(nameof(user));

            // Check if email is already used
            var existingUser = await _userRepository.GetUserByEmailAsync(user.Email);
            if (existingUser != null)
                throw new InvalidOperationException("A user with this email already exists.");

            // Create the user
            await _userRepository.CreateUserAsync(user);
        }

        // Update an existing user
        public async Task UpdateUserAsync(string id, User updatedUser)
        {
            if (string.IsNullOrWhiteSpace(id))
                throw new ArgumentException("Invalid user ID", nameof(id));

            if (updatedUser == null)
                throw new ArgumentNullException(nameof(updatedUser));

            // Check if user exists before updating
            var existingUser = await _userRepository.GetUserByIdAsync(id);
            if (existingUser == null)
                throw new KeyNotFoundException("User not found.");

            // Update only the necessary fields
            existingUser.FirstName = updatedUser.FirstName;
            existingUser.LastName = updatedUser.LastName;
            existingUser.Email = updatedUser.Email;
            existingUser.Role = updatedUser.Role;
            existingUser.IsActive = updatedUser.IsActive;

            // Save changes to the database
            await _userRepository.UpdateUserAsync(id, existingUser);
        }

        // Delete user by ID
        public async Task DeleteUserAsync(string id)
        {
            if (string.IsNullOrWhiteSpace(id))
                throw new ArgumentException("Invalid user ID", nameof(id));

            // Check if user exists before deleting
            var existingUser = await _userRepository.GetUserByIdAsync(id);
            if (existingUser == null)
                throw new KeyNotFoundException("User not found.");

            await _userRepository.DeleteUserAsync(id);
        }

        // Check if a user's email is unique
        public async Task<bool> IsEmailUniqueAsync(string email)
        {
            // Validate email
            if (string.IsNullOrWhiteSpace(email))
                throw new ArgumentException("Email cannot be empty", nameof(email));

            var user = await _userRepository.GetUserByEmailAsync(email);
            return user == null;
        }
        //login
        public async Task<User> LoginAsync(string email, string password)
        {
            
            var user = await _userRepository.GetUserByEmailAsync(email);

            // Validate user 
            if (user == null || user.Password != password)
            {
                throw new UnauthorizedAccessException("Invalid email or password.");
            }

            user.LastLoginDate = DateTime.UtcNow;
            await _userRepository.UpdateUserAsync(user.Id, user);

            return user;
        }
    }
}
