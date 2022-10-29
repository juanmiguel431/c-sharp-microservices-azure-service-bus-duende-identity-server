﻿using Mango.Web.Models;
using Mango.Web.Services.IServices;

namespace Mango.Web.Services;

public class CartService : BaseService, ICartService
{
    public CartService(HttpClient httpClient) : base(httpClient)
    {
    }

    public async Task<T?> GetCartByUserIdAsync<T>(string userId, string token)
    {
        return await SendAsync<T>(new ApiRequest()
        {
            ApiType = Sd.ApiType.Get,
            Url = $"/api/cart/GetCart/{userId}",
            AccessToken = token
        });
    }

    public async Task<T?> AddToCartAsync<T>(CartDto cartDto, string token)
    {
        return await SendAsync<T>(new ApiRequest() {
            ApiType = Sd.ApiType.Post,
            Data = cartDto,
            Url = $"/api/cart/AddCart",
            AccessToken = token
        });
    }

    public async Task<T?> UpdateToCartAsync<T>(CartDto cartDto, string token)
    {
        return await SendAsync<T>(new ApiRequest() {
            ApiType = Sd.ApiType.Put,
            Data = cartDto,
            Url = $"/api/cart/UpdateCart",
            AccessToken = token
        });
    }

    public async Task<T?> RemoveFromCartAsync<T>(int cartId, string token)
    {
        return await SendAsync<T>(new ApiRequest() {
            ApiType = Sd.ApiType.Delete,
            Url = $"/api/cart/RemoveCart",
            AccessToken = token
        });
    }
}