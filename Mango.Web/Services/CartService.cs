using Mango.Web.Models;
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

    public async Task<T?> RemoveFromCartAsync<T>(int cartDetailId, string token)
    {
        return await SendAsync<T>(new ApiRequest() {
            ApiType = Sd.ApiType.Delete,
            Url = $"/api/cart/RemoveCart/{cartDetailId}",
            AccessToken = token
        });
    }

    public async Task<T?> ApplyCoupon<T>(CartDto cartDto, string token)
    {
        return await SendAsync<T>(new ApiRequest {
            ApiType = Sd.ApiType.Post,
            Data = cartDto,
            Url = $"/api/cart/ApplyCoupon",
            AccessToken = token
        });
    }

    public async Task<T?> RemoveCoupon<T>(string userId, string token)
    {
        return await SendAsync<T>(new ApiRequest {
            ApiType = Sd.ApiType.Post,
            Data = userId,
            Url = $"/api/cart/RemoveCoupon",
            AccessToken = token
        });
    }

    public async Task<T?> Checkout<T>(CartHeaderDto cartHeaderDto, string token)
    {
        return await SendAsync<T>(new ApiRequest {
            ApiType = Sd.ApiType.Post,
            Data = cartHeaderDto,
            Url = $"/api/cart/Checkout",
            AccessToken = token
        });
    }
}