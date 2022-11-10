using Mango.Services.ShoppingCartApi.Models.Dtos;
using Newtonsoft.Json;

namespace Mango.Services.ShoppingCartApi.Repositories;

public class CouponRepository : ICouponRepository
{
    private readonly HttpClient _client;

    public CouponRepository(HttpClient client)
    {
        _client = client;
    }
    
    public async Task<CouponDto> GetCoupon(string couponName)
    {
        var response = await _client.GetAsync($"/api/coupon/{couponName}");
        var content = await response.Content.ReadAsStringAsync();
        var resp = JsonConvert.DeserializeObject<ResponseDto>(content);
        if (resp.IsSuccess)
        {
            return JsonConvert.DeserializeObject<CouponDto>(Convert.ToString(resp.Result));
        }

        return new CouponDto();
    }
}