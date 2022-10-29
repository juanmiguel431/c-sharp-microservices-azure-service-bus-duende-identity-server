using Mango.Web.Models;
using Mango.Web.Services.IServices;

namespace Mango.Web.Services;

public class CartService : BaseService, ICartService
{
    public CartService(IHttpClientFactory httpClientFactory) : base(httpClientFactory)
    {
    }

    public async Task<T?> GetCartByUserIdAsync<T>(string userId, string token)
    {
        return await SendAsync<T>(new ApiRequest()
        {
            ApiType = Sd.ApiType.Get,
            Url = $"{Sd.ShoppingCartApiBase}/api/cart/GetCart/{userId}",
            AccessToken = token
        });
    }

    public async Task<T?> AddToCartAsync<T>(CartDto cartDto, string token)
    {
        return await SendAsync<T>(new ApiRequest() {
            ApiType = Sd.ApiType.Post,
            Data = cartDto,
            Url = $"{Sd.ShoppingCartApiBase}/api/cart/AddCart",
            AccessToken = token
        });
    }

    public async Task<T?> UpdateToCartAsync<T>(CartDto cartDto, string token)
    {
        return await SendAsync<T>(new ApiRequest() {
            ApiType = Sd.ApiType.Put,
            Data = cartDto,
            Url = $"{Sd.ShoppingCartApiBase}/api/cart/UpdateCart",
            AccessToken = token
        });
    }

    public async Task<T?> RemoveFromCartAsync<T>(int cartId, string token)
    {
        return await SendAsync<T>(new ApiRequest() {
            ApiType = Sd.ApiType.Delete,
            Url = $"{Sd.ShoppingCartApiBase}/api/cart/RemoveCart",
            AccessToken = token
        });
    }
}