using System.Text;
using Mango.MessageBus;
using Newtonsoft.Json;
using RabbitMQ.Client;

namespace Mango.Services.ShoppingCartApi.RabbitMQSender;

public class RabbitMqCartMessageSender : IRabbitMqCartMessageSender
{
    private readonly string _hostname;
    private readonly string _password;
    private readonly string _username;

    public RabbitMqCartMessageSender()
    {
        _hostname = "localhost";
        _password = "guest";
        _username = "guest";
    }
    
    public void SendMessage(BaseMessage message, string queueName)
    {
        var factory = new ConnectionFactory
        {
            HostName = _hostname,
            UserName = _username,
            Password = _password
        };

        using var connection = factory.CreateConnection();
        using var channel = connection.CreateModel();
        channel.QueueDeclare(queue: queueName, durable: false, exclusive: false, autoDelete: false, arguments: null);

        var json = JsonConvert.SerializeObject(message);
        var body = Encoding.UTF8.GetBytes(json);

        channel.BasicPublish(exchange: string.Empty, routingKey: queueName, basicProperties: null, body: body);
    }
}