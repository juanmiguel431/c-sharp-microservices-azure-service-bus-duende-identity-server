using System.Text;
using Mango.MessageBus;
using Newtonsoft.Json;
using RabbitMQ.Client;

namespace Mango.Services.ShoppingCartApi.RabbitMQSender;

public class RabbitMqCartMessageSender : IRabbitMqCartMessageSender, IDisposable
{
    private readonly IConnection _connection;

    public RabbitMqCartMessageSender()
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
        channel.QueueDeclare(queue: queueName, durable: false, exclusive: false, autoDelete: false, arguments: null);

        var json = JsonConvert.SerializeObject(message);
        var body = Encoding.UTF8.GetBytes(json);

        channel.BasicPublish(exchange: string.Empty, routingKey: queueName, basicProperties: null, body: body);
    }

    public void Dispose()
    {
        _connection?.Dispose();
        GC.SuppressFinalize(this);
    }
}