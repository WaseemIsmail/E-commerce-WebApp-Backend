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

        public async Task CreateProductAsync(Product product)
        {
            await _productRepository.CreateProductAsync(product);
        }

        public async Task<Product> GetProductByIdAsync(string productId)
        {
            return await _productRepository.GetProductByIdAsync(productId);
        }

        public async Task UpdateProductAsync(string productId, Product updatedProduct)
        {
            await _productRepository.UpdateProductAsync(productId, updatedProduct);
        }

        public async Task DeleteProductAsync(string productId)
        {
            await _productRepository.DeleteProductAsync(productId);
        }

        public async Task<IEnumerable<Product>> GetAllProductsAsync()
        {
            return await _productRepository.GetAllProductsAsync();
        }

        // Activate/Deactivate a product (Admin functionality)
        public async Task SetProductStatusAsync(string productId, bool isActive)
        {
            await _productRepository.SetProductStatusAsync(productId, isActive);
        }

        public async Task<IEnumerable<Product>> GetProductsByStatusAsync(bool isActive)
        {
            return await _productRepository.GetProductsByStatusAsync(isActive);
        }

    }
}
