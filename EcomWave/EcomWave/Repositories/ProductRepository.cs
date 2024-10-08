﻿// IT21217586
// Sevindi P.V.D

using EcomWave.Configurations;
using EcomWave.Models;
using MongoDB.Driver;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace EcomWave.Repositories
{
    public class ProductRepository
    {
        private readonly IMongoCollection<Product> _products;

        public ProductRepository(MongoDbContext context)
        {
            _products = context.Products;
        }

        // Create a new product
        public async Task CreateProductAsync(Product product)
        {
            await _products.InsertOneAsync(product);
        }

        // Get product by ID
        public async Task<Product> GetProductByIdAsync(string productId)
        {
            return await _products.Find(p => p.ProductId == productId).FirstOrDefaultAsync();
        }

        // Update a product by ID
        public async Task UpdateProductAsync(string productId, Product updatedProduct)
        {
            var filter = Builders<Product>.Filter.Eq(p => p.ProductId, productId);

            var update = Builders<Product>.Update
                .Set(p => p.Name, updatedProduct.Name)
                .Set(p => p.Description, updatedProduct.Description)
                .Set(p => p.Price, updatedProduct.Price)
                .Set(p => p.IsActive, updatedProduct.IsActive)
                .Set(p => p.UpdatedDate, DateTime.UtcNow);

            await _products.UpdateOneAsync(filter, update);
        }


        // Delete a product by ID
        public async Task DeleteProductAsync(string productId)
        {
            await _products.DeleteOneAsync(p => p.ProductId == productId);
        }

        // Get all products
        public async Task<IEnumerable<Product>> GetAllProductsAsync()
        {
            return await _products.Find(_ => true).ToListAsync();
        }

        // Activate/Deactivate product
        public async Task SetProductStatusAsync(string productId, bool isActive)
        {
            var product = await GetProductByIdAsync(productId);
            if (product != null)
            {
                product.IsActive = isActive;
                await UpdateProductAsync(productId, product);
            }
        }

        // Find all products where IsActive matches the given status
        public async Task<IEnumerable<Product>> GetProductsByStatusAsync(bool isActive)
        {
            return await _products.Find(p => p.IsActive == isActive).ToListAsync();
        }


    }
}
