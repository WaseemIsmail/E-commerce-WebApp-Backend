// IT21217586
// Sevindi P.V.D

using EcomWave.Models;
using EcomWave.Repositories;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace EcomWave.Services
{
    public class ProductService
    {
        private readonly ProductRepository _productRepository;

        public ProductService(ProductRepository productRepository)
        {
            _productRepository = productRepository;
        }

        // Create product
        public async Task CreateProductAsync(Product product)
        {
            await _productRepository.CreateProductAsync(product);
        }

        // Get product by id
        public async Task<Product> GetProductByIdAsync(string productId)
        {
            return await _productRepository.GetProductByIdAsync(productId);
        }

        // Update product
        public async Task UpdateProductAsync(string productId, Product updatedProduct)
        {
            await _productRepository.UpdateProductAsync(productId, updatedProduct);
        }

        // Delete product
        public async Task DeleteProductAsync(string productId)
        {
            await _productRepository.DeleteProductAsync(productId);
        }

        // Get all products
        public async Task<IEnumerable<Product>> GetAllProductsAsync()
        {
            return await _productRepository.GetAllProductsAsync();
        }

        // Activate/Deactivate product
        public async Task SetProductStatusAsync(string productId, bool isActive)
        {
            await _productRepository.SetProductStatusAsync(productId, isActive);
        }

        // Get product by status
        public async Task<IEnumerable<Product>> GetProductsByStatusAsync(bool isActive)
        {
            return await _productRepository.GetProductsByStatusAsync(isActive);
        }

    }
}
