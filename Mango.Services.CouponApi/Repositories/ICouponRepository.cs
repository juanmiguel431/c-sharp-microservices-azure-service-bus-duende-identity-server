using Mango.Services.CouponApi.Models.Dtos;

namespace Mango.Services.CouponApi.Repositories;

public interface ICouponRepository
{
    Task<CouponDto> GetCouponByCode(string couponCode);
}