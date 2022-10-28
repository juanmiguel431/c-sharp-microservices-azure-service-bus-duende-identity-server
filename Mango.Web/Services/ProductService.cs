using Mango.Web.Models;
using Mango.Web.Services.IServices;

namespace Mango.Web.Services;

public class ProductService : BaseService, IProductService
{
    public ProductService(IHttpClientFactory httpClientFactory) : base(httpClientFactory)
    {
    }

    public async Task<T?> CreateProductAsync<T>(ProductDto productDto, string token)
    {
        return await SendAsync<T>(new ApiRequest() {
            ApiType = SD.ApiType.Post,
            Data = productDto,
            Url = $"{SD.ProductApiBase}/api/products",
            AccessToken = token
        });
    }

    public async Task<T?> DeleteProductAsync<T>(int id, string token)
    {
        return await SendAsync<T>(new ApiRequest()
        {
            ApiType = SD.ApiType.Delete,
            Url = $"{SD.ProductApiBase}/api/products/{id}",
            AccessToken = token
        });
    }

    public async Task<T?> GetProductByIdAsync<T>(int id, string token)
    {
        return await SendAsync<T>(new ApiRequest()
        {
            ApiType = SD.ApiType.Get,
            Url = $"{SD.ProductApiBase}/api/products/{id}",
            AccessToken = token
        });
    }

    public async Task<T?> GetAllProductsAsync<T>(string token)
    {
        return await SendAsync<T>(new ApiRequest()
        {
            ApiType = SD.ApiType.Get,
            Url = $"{SD.ProductApiBase}/api/products",
            AccessToken = token
        });
    }

    public async Task<T?> UpdateProductAsync<T>(ProductDto productDto, string token)
    {
        return await SendAsync<T>(new ApiRequest()
        {
            ApiType = SD.ApiType.Put,
            Data = productDto,
            Url = $"{SD.ProductApiBase}/api/products",
            AccessToken = token
        });
    }
}