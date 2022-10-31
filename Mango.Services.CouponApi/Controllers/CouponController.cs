using Mango.Services.CouponApi.Models.Dtos;
using Mango.Services.CouponApi.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace Mango.Services.CouponApi.Controllers;

[ApiController]
[Route("api/coupon")]
public class CouponController : ControllerBase
{
    private readonly ICouponRepository _couponRepository;
    private readonly ResponseDto _response;

    public CouponController(ICouponRepository couponRepository)
    {
        _couponRepository = couponRepository;
        _response = new ResponseDto();
    }

    [HttpGet, Route("{code}")]
    public async Task<ResponseDto> GetDiscountForCode(string code)
    {
        try
        {
            var couponDto = await _couponRepository.GetCouponByCode(code);
            _response.Result = couponDto;
        }
        catch (Exception e)
        {
            _response.IsSuccess = false;
            _response.ErrorMessages = new List<string> { e.Message };
        }

        return _response;
    }
}