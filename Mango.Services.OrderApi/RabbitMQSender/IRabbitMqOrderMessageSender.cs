using Mango.MessageBus;

namespace Mango.Services.OrderApi.RabbitMQSender;

public interface IRabbitMqOrderMessageSender
{
    void SendMessage(BaseMessage baseMessage, string queueName);
}