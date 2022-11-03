using System.Text;
using Azure.Messaging.ServiceBus;
using Mango.Services.OrderApi.Messages;
using Mango.Services.OrderApi.Models;
using Mango.Services.OrderApi.Repositories;
using Newtonsoft.Json;

namespace Mango.Services.OrderApi.Messaging;

public class AzureServiceBusConsumer
{
    private readonly OrderRepository _orderRepository;

    public AzureServiceBusConsumer(OrderRepository orderRepository)
    {
        _orderRepository = orderRepository;
    }
    
    private async Task OnCheckoutMessageReceived(ProcessMessageEventArgs args)
    {
        var message = args.Message;
        var body = Encoding.UTF8.GetString(message.Body);
        var checkoutHeaderDto = JsonConvert.DeserializeObject<CheckoutHeaderDto>(body);

        var orderHeader = new OrderHeader()
        {
            UserId = checkoutHeaderDto.UserId,
            FirstName = checkoutHeaderDto.FirstName,
            LastName = checkoutHeaderDto.LastName,
            OrderDetails = new List<OrderDetail>(),
            CardNumber = checkoutHeaderDto.CardNumber,
            CouponCode = checkoutHeaderDto.CouponCode,
            CVV = checkoutHeaderDto.CVV,
            DiscountTotal = checkoutHeaderDto.DiscountTotal,
            Email = checkoutHeaderDto.Email,
            ExpiryMonthYear = checkoutHeaderDto.ExpiryMonthYear,
            OrderDateTime = DateTime.Now,
            OrderTotal = checkoutHeaderDto.OrderTotal,
            PaymentStatus = false,
            Phone = checkoutHeaderDto.Phone,
            PickupDateTime = checkoutHeaderDto.PickupDateTime,
        };

        foreach (var cartDetail in checkoutHeaderDto.CartDetails)
        {
            var orderDetails = new OrderDetail()
            {
                ProductId = cartDetail.ProductId,
                ProductName = cartDetail.Product.Name,
                Price = cartDetail.Product.Price,
                Count = cartDetail.Count,
            };

            orderHeader.CartTotalItems += cartDetail.Count;
            orderHeader.OrderDetails.Add(orderDetails);
        }

        await _orderRepository.AddOrder(orderHeader);
    }
}