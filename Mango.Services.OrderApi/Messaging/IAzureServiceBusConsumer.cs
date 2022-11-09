namespace Mango.Services.OrderApi.Messaging;

public interface IAzureServiceBusConsumer
{
    Task Start();
    Task Stop();
}