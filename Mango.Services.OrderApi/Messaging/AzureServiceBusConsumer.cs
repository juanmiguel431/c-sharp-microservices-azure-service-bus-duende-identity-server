using System.Text;
using Azure.Messaging.ServiceBus;
using Mango.MessageBus;
using Mango.Services.OrderApi.Messages;
using Mango.Services.OrderApi.Models;
using Mango.Services.OrderApi.Repositories;
using Newtonsoft.Json;

namespace Mango.Services.OrderApi.Messaging;

public class AzureServiceBusConsumer : IAzureServiceBusConsumer
{
    private readonly IOrderRepository _orderRepository;
    private readonly string _orderPaymentProcessTopic;
    private readonly ServiceBusProcessor _checkOutProcessor;
    private readonly ServiceBusProcessor _orderUpdatePaymentStatusProcessor;
    private readonly IMessageBus _messageBus;
    
    public AzureServiceBusConsumer(IOrderRepository orderRepository, IConfiguration configuration, IMessageBus messageBus)
    {
        _orderRepository = orderRepository;
        _messageBus = messageBus;

        var serviceBusConnectionString = configuration.GetValue<string>("AzureServiceBus:ConnectionString");
        var checkoutMessageTopic = configuration.GetValue<string>("AzureServiceBus:CheckoutMessageTopic");
        var mangoOrderSubscription = configuration.GetValue<string>("AzureServiceBus:MangoOrderSubscription");
        
        _orderPaymentProcessTopic = configuration.GetValue<string>("AzureServiceBus:OrderPaymentProcessTopic");
        var orderUpdatePaymentResultTopic = configuration.GetValue<string>("AzureServiceBus:OrderUpdatePaymentResultTopic");

        var client = new ServiceBusClient(serviceBusConnectionString);
        _checkOutProcessor = client.CreateProcessor(checkoutMessageTopic, mangoOrderSubscription);
        _orderUpdatePaymentStatusProcessor = client.CreateProcessor(orderUpdatePaymentResultTopic, mangoOrderSubscription);
    }

    public async Task Start()
    {
        _checkOutProcessor.ProcessMessageAsync += OnCheckoutMessageReceived;
        _checkOutProcessor.ProcessErrorAsync += ErrorHandler;
        await _checkOutProcessor.StartProcessingAsync();
        
        _orderUpdatePaymentStatusProcessor.ProcessMessageAsync += OnOrderPaymentUpdateReceived;
        _orderUpdatePaymentStatusProcessor.ProcessErrorAsync += ErrorHandler;
        await _orderUpdatePaymentStatusProcessor.StartProcessingAsync();
    }
    
    public async Task Stop()
    {
        await _checkOutProcessor.StopProcessingAsync();
        _checkOutProcessor.ProcessMessageAsync -= OnCheckoutMessageReceived;
        _checkOutProcessor.ProcessErrorAsync -= ErrorHandler;
        await _checkOutProcessor.DisposeAsync();
        
        await _orderUpdatePaymentStatusProcessor.StopProcessingAsync();
        _orderUpdatePaymentStatusProcessor.ProcessMessageAsync -= OnOrderPaymentUpdateReceived;
        _orderUpdatePaymentStatusProcessor.ProcessErrorAsync -= ErrorHandler;
        await _orderUpdatePaymentStatusProcessor.DisposeAsync();
    }

    private async Task OnOrderPaymentUpdateReceived(ProcessMessageEventArgs args)
    {
        var message = args.Message;
        var body = Encoding.UTF8.GetString(message.Body);
        var updatePaymentResultMessage = JsonConvert.DeserializeObject<UpdatePaymentResultMessage>(body);

        await _orderRepository.UpdateOrderPaymentStatus(updatePaymentResultMessage.OrderId, updatePaymentResultMessage.Status);
        await args.CompleteMessageAsync(args.Message);
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

        var paymentRequestMessage = new PaymentRequestMessage
        {
            Name = $"{orderHeader.FirstName} {orderHeader.LastName}",
            CardNumber = orderHeader.CardNumber,
            CVV = orderHeader.CVV,
            ExpiryMonthYear = orderHeader.ExpiryMonthYear,
            OrderId = orderHeader.OrderHeaderId,
            OrderTotal = orderHeader.OrderTotal,
        };

        try
        {
            await _messageBus.PublishMessage(paymentRequestMessage, _orderPaymentProcessTopic);
            await args.CompleteMessageAsync(args.Message);
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }
}