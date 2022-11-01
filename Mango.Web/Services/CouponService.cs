using Mango.Web.Models;
using Mango.Web.Services.IServices;

namespace Mango.Web.Services;

public class CouponService : BaseService, ICouponService
{
    public CouponService(HttpClient httpClient) : base(httpClient)
    {
    }

    public async Task<T?> GetCoupon<T>(string couponCode, string token)
    {
        return await SendAsync<T>(new ApiRequest
        {
            ApiType = Sd.ApiType.Get,
            Url = $"/api/coupon/{couponCode}",
            AccessToken = token
        });
    }
}