using System.Text;
using Mango.MessageBus;
using Newtonsoft.Json;
using RabbitMQ.Client;

namespace Mango.Services.PaymentApi.RabbitMQSender;

public class RabbitMqPaymentMessageSender : IRabbitMqPaymentMessageSender, IDisposable
{
    private readonly IConnection _connection;
    private const string ExchangeName = "PublishSubscribePaymentUpdate_Exchange";
    
    private const string ExchangeNameUsingDirectMethod = "DirectPaymentUpdate_Exchange";
    private const string PaymentOrderUpdateQueueName = "PaymentOrderUpdateQueueName";
    private const string PaymentEmailUpdateQueueName = "PaymentEmailUpdateQueueName";
    private const string PaymentOrderRoutingKey = "PaymentOrderRoutingKey";
    private const string PaymentEmailRoutingKey = "PaymentEmailRoutingKey";

    public RabbitMqPaymentMessageSender()
    {
        var factory = new ConnectionFactory
        {
            HostName = "localhost",
            UserName = "guest",
            Password = "guest"
        };

        _connection = factory.CreateConnection();
    }
    
    public void SendMessage(BaseMessage message)
    {
        using var channel = _connection.CreateModel();
        // channel.QueueDeclare(queue: queueName, durable: false, exclusive: false, autoDelete: false, arguments: null);
        // channel.ExchangeDeclare(exchange: ExchangeName, type: ExchangeType.Fanout, durable: false);
        
        channel.ExchangeDeclare(exchange: ExchangeNameUsingDirectMethod, type: ExchangeType.Direct, durable: false);
        channel.QueueDeclare(PaymentOrderUpdateQueueName, false, false, false, null);
        channel.QueueDeclare(PaymentEmailUpdateQueueName, false, false, false, null);
        channel.QueueBind(queue: PaymentOrderUpdateQueueName, exchange: ExchangeNameUsingDirectMethod, routingKey: PaymentOrderRoutingKey);
        channel.QueueBind(queue: PaymentEmailUpdateQueueName, exchange: ExchangeNameUsingDirectMethod, routingKey: PaymentEmailRoutingKey);

        var json = JsonConvert.SerializeObject(message);
        var body = Encoding.UTF8.GetBytes(json);

        // channel.BasicPublish(exchange: ExchangeName, routingKey: string.Empty, basicProperties: null, body: body);
        channel.BasicPublish(exchange: ExchangeNameUsingDirectMethod, routingKey: PaymentOrderRoutingKey, basicProperties: null, body: body);
        channel.BasicPublish(exchange: ExchangeNameUsingDirectMethod, routingKey: PaymentEmailRoutingKey, basicProperties: null, body: body);
    }

    public void Dispose()
    {
        _connection?.Dispose();
        GC.SuppressFinalize(this);
    }
}