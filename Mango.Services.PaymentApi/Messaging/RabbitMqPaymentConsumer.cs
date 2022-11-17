using System.Text;
using Mango.MessageBus;
using Mango.Services.PaymentApi.Messages;
using Mango.Services.PaymentApi.RabbitMQSender;
using Newtonsoft.Json;
using PaymentProcessor;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace Mango.Services.PaymentApi.Messaging;

public class RabbitMqPaymentConsumer : BackgroundService
{
    private readonly IRabbitMqPaymentMessageSender _rabbitMqPaymentMessageSender;
    private readonly IConnection _connection;
    private readonly IModel _channel;
    private readonly IProcessPayment _processPayment;

    public RabbitMqPaymentConsumer(IRabbitMqPaymentMessageSender rabbitMqPaymentMessageSender, IProcessPayment processPayment)
    {
        _rabbitMqPaymentMessageSender = rabbitMqPaymentMessageSender;
        _processPayment = processPayment;

        var factory = new ConnectionFactory
        {
            HostName = "localhost",
            UserName = "guest",
            Password = "guest"
        };

        _connection = factory.CreateConnection();
        _channel = _connection.CreateModel();
        
        _channel.QueueDeclare(queue: QueueName.OrderPaymentProcessQueue, durable: false, exclusive: false, autoDelete: false, arguments: null);
    }

    protected override Task ExecuteAsync(CancellationToken stoppingToken)
    {
        stoppingToken.ThrowIfCancellationRequested();

        var consumer = new EventingBasicConsumer(_channel);

        consumer.Received += (sender, args) =>
        {
            var content = Encoding.UTF8.GetString(args.Body.ToArray());
            
            var checkoutHeaderDto = JsonConvert.DeserializeObject<PaymentRequestMessage>(content);
            HandleMessage(checkoutHeaderDto).GetAwaiter().GetResult();
            
            _channel.BasicAck(args.DeliveryTag, false);
        };

        _channel.BasicConsume(queue: QueueName.OrderPaymentProcessQueue, autoAck: false, consumer: consumer);
        
        return Task.CompletedTask;
    }

    private async Task HandleMessage(PaymentRequestMessage paymentRequestMessage)
    {
        var result = _processPayment.PaymentProcessor();

        var updatePaymentResultMessage = new UpdatePaymentResultMessage
        {
            Status = result,
            OrderId = paymentRequestMessage.OrderId,
            Email = paymentRequestMessage.Email
        };
        
        try
        {
            _rabbitMqPaymentMessageSender.SendMessage(updatePaymentResultMessage, QueueName.OrderUpdatePaymentResultTopic );
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