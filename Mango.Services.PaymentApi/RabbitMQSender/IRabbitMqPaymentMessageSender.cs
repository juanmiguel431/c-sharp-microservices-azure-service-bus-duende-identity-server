using Mango.MessageBus;

namespace Mango.Services.PaymentApi.RabbitMQSender;

public interface IRabbitMqPaymentMessageSender
{
    void SendMessage(BaseMessage baseMessage);
}