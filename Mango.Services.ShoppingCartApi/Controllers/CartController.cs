using Mango.MessageBus;
using Mango.Services.ShoppingCartApi.Messages;
using Mango.Services.ShoppingCartApi.Models.Dtos;
using Mango.Services.ShoppingCartApi.RabbitMQSender;
using Mango.Services.ShoppingCartApi.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace Mango.Services.ShoppingCartApi.Controllers;

[ApiController]
[Route("api/cart")]
public class CartController: ControllerBase
{
    private readonly ICartRepository _cartRepository;
    private readonly ICouponRepository _couponRepository;
    private readonly IConfiguration _configuration;
    private readonly IRabbitMqCartMessageSender _rabbitMqCartMessageSender;
    private readonly IMessageBus _messageBus;
    private readonly ResponseDto _response;

    public CartController(ICartRepository cartRepository, IMessageBus messageBus, ICouponRepository couponRepository, IConfiguration configuration, IRabbitMqCartMessageSender rabbitMqCartMessageSender)
    {
        _cartRepository = cartRepository;
        _messageBus = messageBus;
        _couponRepository = couponRepository;
        _configuration = configuration;
        _rabbitMqCartMessageSender = rabbitMqCartMessageSender;
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
    public async Task<ResponseDto> RemoveCoupon([FromBody] string userId)
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
    
    [HttpPost, Route("Checkout")]
    public async Task<object> Checkout(CheckoutHeaderDto checkoutHeader)
    {
        try
        {
            var cartDto = await _cartRepository.GetCartByUserId(checkoutHeader.UserId);
            if (cartDto == null) return BadRequest();

            if (!string.IsNullOrWhiteSpace(checkoutHeader.CouponCode))
            {
                var coupon = await _couponRepository.GetCoupon(checkoutHeader.CouponCode);
                if (coupon.DiscountAmount != checkoutHeader.DiscountTotal)
                {
                    _response.IsSuccess = false;
                    _response.ErrorMessages = new List<string>() { "Coupon discount has changed, please confirm." };
                    _response.DisplayMessage = "Coupon discount has changed, please confirm.";
                    return _response;
                }
            }
            
            checkoutHeader.CartDetails = cartDto.CartDetails;
            
            // logic to add message to process order.
            // var checkoutMessageTopic = _configuration.GetValue<string>("AzureServiceBus:CheckoutMessageQueue");
            // await _messageBus.PublishMessage(checkoutHeader, checkoutMessageTopic);
            
            //RabbitMQ implementation
            _rabbitMqCartMessageSender.SendMessage(checkoutHeader, QueueName.CheckoutQueue);

            await _cartRepository.ClearCart(checkoutHeader.UserId);
        }
        catch (Exception e)
        {
            _response.IsSuccess = false;
            _response.ErrorMessages = new List<string> { e.Message };
        }

        return _response;
    }
}