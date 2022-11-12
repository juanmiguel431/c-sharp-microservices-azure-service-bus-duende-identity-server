using System.Text;
using Azure.Messaging.ServiceBus;
using Newtonsoft.Json;

namespace Mango.MessageBus;

public class AzureServiceBusMessageBus : IMessageBus
{
    private readonly string _connectionString;

    public AzureServiceBusMessageBus(string connectionString)
    {
        _connectionString = connectionString;
    }
    
    public async Task PublishMessage(BaseMessage message, string topicName)
    {
        await using var client = new ServiceBusClient(_connectionString);
        await using var sender = client.CreateSender(topicName);
        var json = JsonConvert.SerializeObject(message);
        var sbMessage = new ServiceBusMessage(Encoding.UTF8.GetBytes(json))
        {
            CorrelationId = Guid.NewGuid().ToString()
        };
        await sender.SendMessageAsync(sbMessage);
    }
}