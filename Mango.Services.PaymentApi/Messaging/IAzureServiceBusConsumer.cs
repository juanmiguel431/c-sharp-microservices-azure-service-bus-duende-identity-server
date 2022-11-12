namespace Mango.Services.PaymentApi.Messaging;

public interface IAzureServiceBusConsumer
{
    Task Start();
    Task Stop();
}