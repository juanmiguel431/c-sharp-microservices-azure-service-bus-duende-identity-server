using Mango.Web.Models;

namespace Mango.Web.Services.IServices;

public interface ICartService : IBaseService
{
    Task<T?> GetCartByUserIdAsync<T>(string userId, string token);
    Task<T?> AddToCartAsync<T>(CartDto cartDto, string token);
    Task<T?> UpdateToCartAsync<T>(CartDto cartDto, string token);
    Task<T?> RemoveFromCartAsync<T>(int cartId, string token);
}