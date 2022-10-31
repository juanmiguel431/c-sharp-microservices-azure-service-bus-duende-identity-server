using Mango.Services.ShoppingCartApi.Models.Dtos;
using Mango.Services.ShoppingCartApi.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace Mango.Services.ShoppingCartApi.Controllers;

[ApiController]
[Route("api/cart")]
public class CartController: ControllerBase
{
    private readonly ICartRepository _cartRepository;
    private readonly ResponseDto _response;

    public CartController(ICartRepository cartRepository)
    {
        _cartRepository = cartRepository;
        _response = new ResponseDto();
    }

    [HttpGet, Route("GetCart/{userId}")]
    public async Task<ResponseDto> GetCart(string userId)
    {
        try
        {
            var cartDto = await _cartRepository.GetCartByUserId(userId);
            _response.Result = cartDto;
        }
        catch (Exception e)
        {
            _response.IsSuccess = false;
            _response.ErrorMessages = new List<string> { e.Message };
        }

        return _response;
    }
    
    [HttpPost, Route("AddCart")]
    public async Task<ResponseDto> AddCart(CartDto cartDto)
    {
        try
        {
            var cartDt = await _cartRepository.CreateUpdateCart(cartDto);
            _response.Result = cartDt;
        }
        catch (Exception e)
        {
            _response.IsSuccess = false;
            _response.ErrorMessages = new List<string> { e.Message };
        }

        return _response;
    }
    
    [HttpPut, Route("UpdateCart")]
    public async Task<ResponseDto> UpdateCart(CartDto cartDto)
    {
        try
        {
            var cartDt = await _cartRepository.CreateUpdateCart(cartDto);
            _response.Result = cartDt;
        }
        catch (Exception e)
        {
            _response.IsSuccess = false;
            _response.ErrorMessages = new List<string> { e.Message };
        }

        return _response;
    }
    
    [HttpDelete, Route("RemoveCart/{cartDetailId}")]
    public async Task<ResponseDto> UpdateCart(int cartDetailId)
    {
        try
        {
            var isSuccess = await _cartRepository.RemoveFromCart(cartDetailId);
            _response.Result = isSuccess;
        }
        catch (Exception e)
        {
            _response.IsSuccess = false;
            _response.ErrorMessages = new List<string> { e.Message };
        }

        return _response;
    }
    
    [HttpPost, Route("ApplyCoupon")]
    public async Task<ResponseDto> ApplyCoupon(CartDto cartDto)
    {
        try
        {
            var isSuccess = await _cartRepository.ApplyCoupon(cartDto.CartHeader.UserId, cartDto.CartHeader.CouponCode);
            _response.Result = isSuccess;
        }
        catch (Exception e)
        {
            _response.IsSuccess = false;
            _response.ErrorMessages = new List<string> { e.Message };
        }

        return _response;
    }
    
    [HttpPost, Route("RemoveCoupon")]
    public async Task<ResponseDto> RemoveCoupon(string userId)
    {
        try
        {
            var isSuccess = await _cartRepository.RemoveCoupon(userId);
            _response.Result = isSuccess;
        }
        catch (Exception e)
        {
            _response.IsSuccess = false;
            _response.ErrorMessages = new List<string> { e.Message };
        }

        return _response;
    }
}