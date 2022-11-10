using Mango.Services.ShoppingCartApi.Models.Dtos;

namespace Mango.Services.ShoppingCartApi.Repositories;

public interface ICouponRepository
{
    Task<CouponDto> GetCoupon(string couponName);
}