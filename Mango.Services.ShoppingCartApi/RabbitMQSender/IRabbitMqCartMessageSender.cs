using Mango.MessageBus;

namespace Mango.Services.ShoppingCartApi.RabbitMQSender;

public interface IRabbitMqCartMessageSender
{
    void SendMessage(BaseMessage baseMessage, string queueName);
}