using System.Text;
using Mango.MessageBus;
using Mango.Services.OrderApi.Messages;
using Mango.Services.OrderApi.Models;
using Mango.Services.OrderApi.Repositories;
using Newtonsoft.Json;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace Mango.Services.OrderApi.Messaging;

public class RabbitMqCheckOutConsumer : BackgroundService
{
    private readonly IOrderRepository _orderRepository;
    private readonly IConnection _connection;
    private readonly IModel _channel;

    public RabbitMqCheckOutConsumer(IOrderRepository orderRepository)
    {
        _orderRepository = orderRepository;
        
        var factory = new ConnectionFactory
        {
            HostName = "localhost",
            UserName = "guest",
            Password = "guest"
        };

        _connection = factory.CreateConnection();
        _channel = _connection.CreateModel();
        
        _channel.QueueDeclare(queue: QueueName.CheckoutQueue, durable: false, exclusive: false, autoDelete: false, arguments: null);
    }

    protected override Task ExecuteAsync(CancellationToken stoppingToken)
    {
        stoppingToken.ThrowIfCancellationRequested();

        var consumer = new EventingBasicConsumer(_channel);

        consumer.Received += (sender, args) =>
        {
            var content = Encoding.UTF8.GetString(args.Body.ToArray());
            
            var checkoutHeaderDto = JsonConvert.DeserializeObject<CheckoutHeaderDto>(content);
            HandleMessage(checkoutHeaderDto).GetAwaiter().GetResult();
            
            _channel.BasicAck(args.DeliveryTag, false);
        };

        _channel.BasicConsume(queue: QueueName.CheckoutQueue, autoAck: false, consumer: consumer);
        
        return Task.CompletedTask;
    }

    private async Task HandleMessage(CheckoutHeaderDto checkoutHeaderDto)
    {
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
            Email = orderHeader.Email
        };

        try
        {
            // await _messageBus.PublishMessage(paymentRequestMessage, _orderPaymentProcessTopic);
            // await args.CompleteMessageAsync(args.Message);
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }

    public override void Dispose()
    {
        _channel?.Dispose();
        _connection?.Dispose();
        base.Dispose();
        GC.SuppressFinalize(this);
    }
}