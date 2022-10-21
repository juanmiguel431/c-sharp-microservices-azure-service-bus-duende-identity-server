using Mango.Web.Models;
using Mango.Web.Services.IServices;

namespace Mango.Web.Services
{
    public class ProductService : BaseService, IProductService
    {
        public ProductService(IHttpClientFactory httpClientFactory) : base(httpClientFactory)
        {
        }

        public async Task<T?> CreateProductAsync<T>(ProductDto productDto)
        {
            return await SendAsync<T>(new ApiRequest() {
                ApiType = SD.ApiType.Post,
                Data = productDto,
                Url = $"{SD.ProductApiBase}/api/products",
                AccessToken = String.Empty
            });
        }

        public async Task<T?> DeleteProductAsync<T>(int id)
        {
            return await SendAsync<T>(new ApiRequest()
            {
                ApiType = SD.ApiType.Delete,
                Url = $"{SD.ProductApiBase}/api/products/{id}",
                AccessToken = String.Empty
            });
        }

        public async Task<T?> GetAllProductByIdAsync<T>(int id)
        {
            return await SendAsync<T>(new ApiRequest()
            {
                ApiType = SD.ApiType.Get,
                Url = $"{SD.ProductApiBase}/api/products/{id}",
                AccessToken = String.Empty
            });
        }

        public async Task<T?> GetAllProductsAsync<T>()
        {
            return await SendAsync<T>(new ApiRequest()
            {
                ApiType = SD.ApiType.Get,
                Url = $"{SD.ProductApiBase}/api/products",
                AccessToken = String.Empty
            });
        }

        public async Task<T?> UpdateProductAsync<T>(ProductDto productDto)
        {
            return await SendAsync<T>(new ApiRequest()
            {
                ApiType = SD.ApiType.Put,
                Data = productDto,
                Url = $"{SD.ProductApiBase}/api/products",
                AccessToken = String.Empty
            });
        }
    }
}
