using System.Text;
using Mango.Services.OrderApi.Messages;
using Mango.Services.OrderApi.Repositories;
using Newtonsoft.Json;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace Mango.Services.OrderApi.Messaging;

public class RabbitMqPaymentUpdateConsumer : BackgroundService
{
    private readonly IOrderRepository _orderRepository;
    private readonly IConnection _connection;
    private readonly IModel _channel;
    private const string ExchangeName = "PublishSubscribePaymentUpdate_Exchange";
    private readonly string _queueName;
    
    private const string ExchangeNameUsingDirectMethod = "DirectPaymentUpdate_Exchange";
    private const string PaymentOrderUpdateQueueName = "PaymentOrderUpdateQueueName";
    private const string PaymentOrderRoutingKey = "PaymentOrderRoutingKey";
    
    public RabbitMqPaymentUpdateConsumer(IOrderRepository orderRepository)
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
        
        // _channel.ExchangeDeclare(exchange: ExchangeName, type: ExchangeType.Fanout);
        _channel.ExchangeDeclare(exchange: ExchangeNameUsingDirectMethod, type: ExchangeType.Direct);
        
        // _queueName = _channel.QueueDeclare().QueueName;
        _queueName = _channel.QueueDeclare(PaymentOrderUpdateQueueName, false, false, false, null).QueueName;
        
        // _channel.QueueBind(_queueName, ExchangeName, string.Empty);
        _channel.QueueBind(_queueName, ExchangeNameUsingDirectMethod, PaymentOrderRoutingKey);
    }

    protected override Task ExecuteAsync(CancellationToken stoppingToken)
    {
        stoppingToken.ThrowIfCancellationRequested();

        var consumer = new EventingBasicConsumer(_channel);

        consumer.Received += (sender, args) =>
        {
            var content = Encoding.UTF8.GetString(args.Body.ToArray());
            
            var updatePaymentResultMessage = JsonConvert.DeserializeObject<UpdatePaymentResultMessage>(content);
            HandleMessage(updatePaymentResultMessage).GetAwaiter().GetResult();
            
            _channel.BasicAck(args.DeliveryTag, false);
        };

        _channel.BasicConsume(queue: _queueName, autoAck: false, consumer: consumer);
        
        return Task.CompletedTask;
    }

    private async Task HandleMessage(UpdatePaymentResultMessage updatePaymentResultMessage)
    {
        await _orderRepository.UpdateOrderPaymentStatus(updatePaymentResultMessage.OrderId, updatePaymentResultMessage.Status);
    }

    public override void Dispose()
    {
        _channel?.Dispose();
        _connection?.Dispose();
        base.Dispose();
        GC.SuppressFinalize(this);
    }
}