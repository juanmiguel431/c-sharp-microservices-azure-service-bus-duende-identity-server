using System.Text;
using Mango.Services.Email.Messages;
using Mango.Services.Email.Repositories;
using Newtonsoft.Json;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace Mango.Services.Email.Messaging;

public class RabbitMqEmailConsumer : BackgroundService
{
    private readonly IEmailRepository _emailRepository;
    private readonly IConnection _connection;
    private readonly IModel _channel;
    private const string ExchangeName = "PublishSubscribePaymentUpdate_Exchange";
    private readonly string _queueName;
    
    public RabbitMqEmailConsumer(IEmailRepository emailRepository)
    {
        _emailRepository = emailRepository;
        var factory = new ConnectionFactory
        {
            HostName = "localhost",
            UserName = "guest",
            Password = "guest"
        };

        _connection = factory.CreateConnection();
        _channel = _connection.CreateModel();
        
        _channel.ExchangeDeclare(exchange: ExchangeName, type: ExchangeType.Fanout);
        _queueName = _channel.QueueDeclare().QueueName;
        _channel.QueueBind(_queueName, ExchangeName, string.Empty);
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
        await _emailRepository.SendAndLogEmail(updatePaymentResultMessage);
    }

    public override void Dispose()
    {
        _channel?.Dispose();
        _connection?.Dispose();
        base.Dispose();
        GC.SuppressFinalize(this);
    }
}