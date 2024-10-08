// IT21215988
// Waseem M.I.M

using EcomWave.Models;
using MongoDB.Driver;

namespace EcomWave.Configurations
{
    public class MongoDbContext
    {
        private readonly IMongoDatabase _database;

        // Get database connection
        public MongoDbContext(MongoDbConfig config)
        {
            var client = new MongoClient(config.ConnectionString);
            _database = client.GetDatabase(config.DatabaseName);
        }

        public IMongoCollection<User> Users => _database.GetCollection<User>("Users");
        public IMongoCollection<Product> Products => _database.GetCollection<Product>("Products");
        public IMongoCollection<Order> Orders => _database.GetCollection<Order>("Orders");
        public IMongoCollection<Inventory> Inventory => _database.GetCollection<Inventory>("Inventory");

    }
}
