// IT21215988
// Waseem M.I.M

using EcomWave.Models;
using EcomWave.Configurations;
using MongoDB.Driver;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Security.Claims;
using Microsoft.EntityFrameworkCore;

namespace EcomWave.Repositories
{
    public class UserRepository
    {
        private readonly IMongoCollection<User> _users;

        public UserRepository(MongoDbContext context)
        {
            _users = context.Users;
        }

        // Create a new user in the Users collection
        public async Task CreateUserAsync(User user)
        {
            await _users.InsertOneAsync(user);
        }

        // Get user by email and password (login validation)
        public async Task<User> GetUserByEmailAndPasswordAsync(string email, string password)
        {
            return await _users.Find(u => u.Email == email && u.Password == password).FirstOrDefaultAsync();
        }

        // Get a user by email
        public async Task<User> GetUserByEmailAsync(string email)
        {
            return await _users.Find(u => u.Email == email).FirstOrDefaultAsync();
        }

        // Get a user by ID
        public async Task<User> GetUserByIdAsync(string userId)
        {
            return await _users.Find(u => u.UserId == userId).FirstOrDefaultAsync();
        }

        //get users by role
        public async Task<IEnumerable<User>> GetUsersByRoleAsync(UserRole role)
        {
            return await _users.Find(u => u.Role == role).ToListAsync();
        }

        // Get all users (Admin only)
        public async Task<IEnumerable<User>> GetAllUsersAsync()
        {
            return await _users.Find(_ => true).ToListAsync();
        }

        // Update user information
        public async Task UpdateUserAsync(User user)
        {
            var filter = Builders<User>.Filter.Eq(u => u.UserId, user.UserId);
            await _users.ReplaceOneAsync(filter, user);
        }

        // Deactivate user account
        public async Task DeactivateUserAsync(string userId)
        {
            var update = Builders<User>.Update.Set(u => u.IsActive, false);
            await _users.UpdateOneAsync(u => u.UserId == userId, update);
        }

        // Reactivate user account (CSR only)
        public async Task ReactivateUserAsync(string userId)
        {
            var update = Builders<User>.Update.Set(u => u.IsActive, true);
            await _users.UpdateOneAsync(u => u.UserId == userId, update);
        }

        // Add a rating and comment to a vendor
        public async Task AddVendorRatingAsync(string vendorId, VendorRating rating)
        {
            var filter = Builders<User>.Filter.Eq(u => u.UserId, vendorId);
            var vendor = await _users.Find(filter).FirstOrDefaultAsync();

            if (vendor.VendorInfo == null)
            {
                vendor.VendorInfo = new VendorDetails
                {
                    Ratings = new List<VendorRating>()
                };
            }

            vendor.VendorInfo.Ratings.Add(rating);

            var update = Builders<User>.Update.Set(u => u.VendorInfo, vendor.VendorInfo);
            await _users.UpdateOneAsync(filter, update);
        }


        // Calculate and update the average rating for a vendor
        public async Task UpdateAverageRatingAsync(string vendorId)
        {
            var filter = Builders<User>.Filter.Eq(u => u.UserId, vendorId);
            var vendor = await _users.Find(filter).FirstOrDefaultAsync();

            if (vendor?.VendorInfo?.Ratings != null && vendor.VendorInfo.Ratings.Count > 0)
            {
                var averageRating = vendor.VendorInfo.Ratings.Average(r => r.Rating);

                var update = Builders<User>.Update.Set("VendorInfo.AverageRating", averageRating);
                await _users.UpdateOneAsync(filter, update);
            }
        }

        // Update comment for a specific customer
        public async Task UpdateVendorCommentAsync(string vendorId, string customerId, string newComment)
        {
            var filter = Builders<User>.Filter.And(
                Builders<User>.Filter.Eq(u => u.UserId, vendorId),
                Builders<User>.Filter.ElemMatch(u => u.VendorInfo.Ratings, r => r.CustomerId == customerId)
            );
            var update = Builders<User>.Update.Set("VendorInfo.Ratings.$.Comment", newComment);
            await _users.UpdateOneAsync(filter, update);
        }

       


    }
}
