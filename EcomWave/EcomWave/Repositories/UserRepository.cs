using EcomWave.Configurations;
using EcomWave.Models;
using MongoDB.Driver;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace EcomWave.Repositories
{
    public class UserRepository
    {
        private readonly IMongoCollection<User> _users;

        public UserRepository(MongoDbContext context)
        {
            _users = context.Users;
        }

        // Get all users
        public async Task<IEnumerable<User>> GetAllUsersAsync()
        {
            return await _users.Find(_ => true).ToListAsync();
        }

        // Get user by ID
        public async Task<User?> GetUserByIdAsync(string id)
        {
            // Find user by ID, return null if not found
            return await _users.Find(u => u.Id == id).FirstOrDefaultAsync();
        }

        // Create a new user
        public async Task CreateUserAsync(User user)
        {
            // Insert the new user document
            await _users.InsertOneAsync(user);
        }

        // Update an existing user
        public async Task UpdateUserAsync(string id, User updatedUser)
        {
            // Replace the entire user document with the updated one
            await _users.ReplaceOneAsync(u => u.Id == id, updatedUser);
        }

        // Delete a user by ID
        public async Task DeleteUserAsync(string id)
        {
            // Remove the user document by ID
            await _users.DeleteOneAsync(u => u.Id == id);
        }

        // Find user by email
        public async Task<User?> GetUserByEmailAsync(string email)
        {
            // Find user by email, return null if not found
            return await _users.Find(u => u.Email == email).FirstOrDefaultAsync();
        }
    }
}
