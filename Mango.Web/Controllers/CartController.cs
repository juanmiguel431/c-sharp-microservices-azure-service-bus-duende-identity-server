using Mango.Web.Models;
using Mango.Web.Services.IServices;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace Mango.Web.Controllers;

public class CartController : Controller
{
    private readonly IProductService _productService;
    private readonly ICartService _cartService;
    private readonly ICouponService _couponService;
    private const string AccessToken = "access_token";

    public CartController(IProductService productService, ICartService cartService, ICouponService couponService)
    {
        _productService = productService;
        _cartService = cartService;
        _couponService = couponService;
    }

    [Authorize]
    public async Task<IActionResult> CartIndex()
    {
        var cartDto = await LoadCartDto();
        return View(cartDto);
    }

    private async Task<CartDto> LoadCartDto()
    {
        var userId = GetUserId();
        var token = await GetToken();

        var response = await _cartService.GetCartByUserIdAsync<ResponseDto>(userId, token);

        CartDto? cartDto;
        if (response != null && response.IsSuccess)
        {
            cartDto = JsonConvert.DeserializeObject<CartDto>(Convert.ToString(response.Result));
        }
        else
        {
            cartDto = new CartDto();
        }

        if (cartDto.CartHeader != null)
        {
            if (!string.IsNullOrEmpty(cartDto.CartHeader.CouponCode))
            {
                var couponResponse = await _couponService.GetCoupon<ResponseDto>(cartDto.CartHeader.CouponCode, token);
                if (couponResponse != null && couponResponse.IsSuccess)
                {
                    var coupon = JsonConvert.DeserializeObject<CouponDto>(Convert.ToString(couponResponse.Result));
                    cartDto.CartHeader.DiscountTotal = coupon.DiscountAmount;
                }
            }
            
            foreach (var detail in cartDto.CartDetails)
            {
                cartDto.CartHeader.OrderTotal += (detail.Product.Price * detail.Count);
            }

            cartDto.CartHeader.OrderTotal -= cartDto.CartHeader.DiscountTotal;
        }

        return cartDto;
    }
    
    private string GetUserId()
    {
        var claim = User.Claims.Single(u => u.Type == "sub");
        return claim.Value;
    }
    
    private async Task<string?> GetToken()
    {
        return await HttpContext.GetTokenAsync(AccessToken);
    }

    public async Task<IActionResult> Remove(int cartDetailId)
    {
        var token = await GetToken();
        var response = await _cartService.RemoveFromCartAsync<ResponseDto>(cartDetailId, token);
        return RedirectToAction(nameof(CartIndex));
    }

    [HttpPost]
    public async Task<IActionResult> ApplyCoupon(CartDto cartDto)
    {
        var token = await GetToken();
        var response = await _cartService.ApplyCoupon<ResponseDto>(cartDto, token);
        return RedirectToAction(nameof(CartIndex));
    }

    [HttpPost]
    public async Task<IActionResult> RemoveCoupon(CartDto cartDto)
    {
        var token = await GetToken();
        var response = await _cartService.RemoveCoupon<ResponseDto>(cartDto.CartHeader.UserId, token);
        return RedirectToAction(nameof(CartIndex));
    }
}