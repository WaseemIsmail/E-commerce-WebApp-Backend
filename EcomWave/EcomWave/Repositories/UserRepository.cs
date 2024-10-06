using EcomWave.Models;
using EcomWave.Configurations;
using MongoDB.Driver;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace EcomWave.Repositories
{
    public class UserRepository
    {
        private readonly MongoDbContext _context;

        public UserRepository(MongoDbContext context)
        {
            _context = context;
        }

        // Create a new user in the Users collection
        public async Task CreateUserAsync(User user)
        {
            await _context.Users.InsertOneAsync(user);
        }

        // Get user by email and password (login validation)
        public async Task<User> GetUserByEmailAndPasswordAsync(string email, string password)
        {
            return await _context.Users.Find(u => u.Email == email && u.Password == password).FirstOrDefaultAsync();
        }

        // Get a user by email
        public async Task<User> GetUserByEmailAsync(string email)
        {
            return await _context.Users.Find(u => u.Email == email).FirstOrDefaultAsync();
        }

        // Get a user by ID
        public async Task<User> GetUserByIdAsync(string userId)
        {
            return await _context.Users.Find(u => u.UserId == userId).FirstOrDefaultAsync();
        }

        // Get all users (Admin only)
        public async Task<IEnumerable<User>> GetAllUsersAsync()
        {
            return await _context.Users.Find(_ => true).ToListAsync();
        }

        //// Update user information
        //public async Task UpdateUserAsync(User user)
        //{
        //    await _context.Users.ReplaceOneAsync(u => u.UserId == user.UserId, user);
        //}

        // Deactivate user account
        public async Task DeactivateUserAsync(string userId)
        {
            var update = Builders<User>.Update.Set(u => u.IsActive, false);
            await _context.Users.UpdateOneAsync(u => u.UserId == userId, update);
        }

        // Reactivate user account (CSR only)
        public async Task ReactivateUserAsync(string userId)
        {
            var update = Builders<User>.Update.Set(u => u.IsActive, true);
            await _context.Users.UpdateOneAsync(u => u.UserId == userId, update);
        }

        public async Task UpdateUserAsync(User user)
        {
            var filter = Builders<User>.Filter.Eq(u => u.UserId, user.UserId);
            await _context.Users.ReplaceOneAsync(filter, user);
        }

    }
}
