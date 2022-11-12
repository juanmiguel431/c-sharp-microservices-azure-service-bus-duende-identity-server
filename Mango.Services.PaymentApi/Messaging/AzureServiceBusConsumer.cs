using System.Text;
using Azure.Messaging.ServiceBus;
using Mango.MessageBus;
using Mango.Services.PaymentApi.Messages;
using Newtonsoft.Json;
using PaymentProcessor;

namespace Mango.Services.PaymentApi.Messaging;

public class AzureServiceBusConsumer : IAzureServiceBusConsumer
{
    private readonly string _orderUpdatePaymentResultTopic;
    private readonly ServiceBusProcessor _orderPaymentProcessor;
    private readonly IMessageBus _messageBus;
    private readonly IProcessPayment _processPayment;
        
    public AzureServiceBusConsumer(IConfiguration configuration, IMessageBus messageBus, IProcessPayment processPayment)
    {
        _messageBus = messageBus;
        _processPayment = processPayment;

        var serviceBusConnectionString = configuration.GetValue<string>("AzureServiceBus:ConnectionString");
        var mangoOrderSubscription = configuration.GetValue<string>("AzureServiceBus:MangoOrderSubscription");
        
        var orderPaymentProcessTopic = configuration.GetValue<string>("AzureServiceBus:OrderPaymentProcessTopic");
        _orderUpdatePaymentResultTopic = configuration.GetValue<string>("AzureServiceBus:OrderUpdatePaymentResultTopic");

        var client = new ServiceBusClient(serviceBusConnectionString);
        _orderPaymentProcessor = client.CreateProcessor(orderPaymentProcessTopic, mangoOrderSubscription);
    }

    public async Task Start()
    {
        _orderPaymentProcessor.ProcessMessageAsync += OnProcessPayments;
        _orderPaymentProcessor.ProcessErrorAsync += ErrorHandler;
        await _orderPaymentProcessor.StartProcessingAsync();
    }
    
    public async Task Stop()
    {
        await _orderPaymentProcessor.StopProcessingAsync();
        _orderPaymentProcessor.ProcessMessageAsync -= OnProcessPayments;
        _orderPaymentProcessor.ProcessErrorAsync -= ErrorHandler;
        await _orderPaymentProcessor.DisposeAsync();
    }

    private async Task ErrorHandler(ProcessErrorEventArgs arg)
    {
        Console.WriteLine(arg.Exception.ToString());
        await Task.CompletedTask;
    }

    private async Task OnProcessPayments(ProcessMessageEventArgs args)
    {
        var message = args.Message;
        var body = Encoding.UTF8.GetString(message.Body);
        var paymentRequestMessage = JsonConvert.DeserializeObject<PaymentRequestMessage>(body);

        var result = _processPayment.PaymentProcessor();

        var updatePaymentResultMessage = new UpdatePaymentResultMessage
        {
            Status = result,
            OrderId = paymentRequestMessage.OrderId
        };

        try
        {
            await _messageBus.PublishMessage(updatePaymentResultMessage, _orderUpdatePaymentResultTopic);
            await args.CompleteMessageAsync(args.Message);
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }
}