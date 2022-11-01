using Mango.Web.Models;

namespace Mango.Web.Services.IServices;

public interface ICouponService : IBaseService
{
    Task<T?> GetCoupon<T>(string couponCode, string token);
}