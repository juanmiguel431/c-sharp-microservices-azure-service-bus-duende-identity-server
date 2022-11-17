using System.Text;
using Mango.MessageBus;
using Newtonsoft.Json;
using RabbitMQ.Client;

namespace Mango.Services.PaymentApi.RabbitMQSender;

public class RabbitMqPaymentMessageSender : IRabbitMqPaymentMessageSender, IDisposable
{
    private readonly IConnection _connection;
    private const string ExchangeName = "PublishSubscribePaymentUpdate_Exchange";

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
    
    public void SendMessage(BaseMessage message, string queueName)
    {
        using var channel = _connection.CreateModel();
        // channel.QueueDeclare(queue: queueName, durable: false, exclusive: false, autoDelete: false, arguments: null);
        channel.ExchangeDeclare(exchange: ExchangeName, type: ExchangeType.Fanout, durable: false);

        var json = JsonConvert.SerializeObject(message);
        var body = Encoding.UTF8.GetBytes(json);

        channel.BasicPublish(exchange: ExchangeName, routingKey: string.Empty, basicProperties: null, body: body);
    }

    public void Dispose()
    {
        _connection?.Dispose();
        GC.SuppressFinalize(this);
    }
}