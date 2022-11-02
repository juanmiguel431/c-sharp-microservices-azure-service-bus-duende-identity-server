using System.Text;
using Azure.Messaging.ServiceBus;
using Newtonsoft.Json;

namespace Mango.MessageBus;

public class AzureServiceBusMessageBus : IMessageBus
{
    private const string ConnectionString = "Endpoint=sb://mango-restaurant-jmpc.servicebus.windows.net/;SharedAccessKeyName=RootManageSharedAccessKey;SharedAccessKey=4EXF4ImN0mheZnevDydMmjS70Bv5XbLwH9IQy0thuzE=";
    
    public async Task PublishMessage(BaseMessage message, string topicName)
    {
        await using var client = new ServiceBusClient(ConnectionString);
        await using var sender = client.CreateSender("checkout-message-topic");
        var json = JsonConvert.SerializeObject(message);
        var sbMessage = new ServiceBusMessage(Encoding.UTF8.GetBytes(json))
        {
            CorrelationId = Guid.NewGuid().ToString()
        };
        await sender.SendMessageAsync(sbMessage);
    }
}