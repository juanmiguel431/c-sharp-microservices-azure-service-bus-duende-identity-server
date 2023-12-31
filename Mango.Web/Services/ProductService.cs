﻿using Mango.Web.Models;
using Mango.Web.Services.IServices;

namespace Mango.Web.Services;

public class ProductService : BaseService, IProductService
{
    public ProductService(HttpClient httpClient) : base(httpClient)
    {
    }

    public async Task<T?> CreateProductAsync<T>(ProductDto productDto, string token)
    {
        return await SendAsync<T>(new ApiRequest() {
            ApiType = Sd.ApiType.Post,
            Data = productDto,
            Url = $"/api/products",
            AccessToken = token
        });
    }

    public async Task<T?> DeleteProductAsync<T>(int id, string token)
    {
        return await SendAsync<T>(new ApiRequest()
        {
            ApiType = Sd.ApiType.Delete,
            Url = $"/api/products/{id}",
            AccessToken = token
        });
    }

    public async Task<T?> GetProductByIdAsync<T>(int id, string token)
    {
        return await SendAsync<T>(new ApiRequest()
        {
            ApiType = Sd.ApiType.Get,
            Url = $"/api/products/{id}",
            AccessToken = token
        });
    }

    public async Task<T?> GetAllProductsAsync<T>(string token)
    {
        return await SendAsync<T>(new ApiRequest()
        {
            ApiType = Sd.ApiType.Get,
            Url = $"/api/products",
            AccessToken = token
        });
    }

    public async Task<T?> UpdateProductAsync<T>(ProductDto productDto, string token)
    {
        return await SendAsync<T>(new ApiRequest()
        {
            ApiType = Sd.ApiType.Put,
            Data = productDto,
            Url = $"/api/products",
            AccessToken = token
        });
    }
}