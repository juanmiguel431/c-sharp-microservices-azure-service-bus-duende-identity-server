using System.Text;
using Azure.Messaging.ServiceBus;
using Mango.Services.OrderApi.Messages;
using Mango.Services.OrderApi.Models;
using Mango.Services.OrderApi.Repositories;
using Newtonsoft.Json;

namespace Mango.Services.OrderApi.Messaging;

public class AzureServiceBusConsumer : IAzureServiceBusConsumer
{
    private readonly IOrderRepository _orderRepository;
    private readonly string _serviceBusConnectionString;
    private readonly string _checkoutMessageTopic;
    private readonly string _mangoOrderSubscription;

    private readonly ServiceBusProcessor _checkOutProcessor;
        
    public AzureServiceBusConsumer(IOrderRepository orderRepository, IConfiguration configuration)
    {
        _orderRepository = orderRepository;

        _serviceBusConnectionString = configuration.GetValue<string>("AzureServiceBus:ConnectionString");
        _checkoutMessageTopic = configuration.GetValue<string>("AzureServiceBus:CheckoutMessageTopic");
        _mangoOrderSubscription = configuration.GetValue<string>("AzureServiceBus:MangoOrderSubscription");

        var client = new ServiceBusClient(_serviceBusConnectionString);
        _checkOutProcessor = client.CreateProcessor(_checkoutMessageTopic, _mangoOrderSubscription);
    }

    public async Task Start()
    {
        _checkOutProcessor.ProcessMessageAsync += OnCheckoutMessageReceived;
        _checkOutProcessor.ProcessErrorAsync += ErrorHandler;
        await _checkOutProcessor.StartProcessingAsync();
    }
    
    public async Task Stop()
    {
        await _checkOutProcessor.StopProcessingAsync();
        _checkOutProcessor.ProcessMessageAsync -= OnCheckoutMessageReceived;
        _checkOutProcessor.ProcessErrorAsync -= ErrorHandler;
        await _checkOutProcessor.DisposeAsync();
    }

    private async Task ErrorHandler(ProcessErrorEventArgs arg)
    {
        Console.WriteLine(arg.Exception.ToString());
        await Task.CompletedTask;
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