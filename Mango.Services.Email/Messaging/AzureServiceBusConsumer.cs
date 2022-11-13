using System.Text;
using Azure.Messaging.ServiceBus;
using Mango.Services.Email.Messages;
using Mango.Services.Email.Repositories;
using Newtonsoft.Json;

namespace Mango.Services.Email.Messaging;

public class AzureServiceBusConsumer : IAzureServiceBusConsumer
{
    private readonly IEmailRepository _emailRepository;
    private readonly ServiceBusProcessor _orderUpdatePaymentStatusProcessor;

    public AzureServiceBusConsumer(IEmailRepository emailRepository, IConfiguration configuration)
    {
        _emailRepository = emailRepository;

        var serviceBusConnectionString = configuration.GetValue<string>("AzureServiceBus:ConnectionString");
        var orderUpdatePaymentResultTopic = configuration.GetValue<string>("AzureServiceBus:OrderUpdatePaymentResultTopic");
        var emailSubscription = configuration.GetValue<string>("AzureServiceBus:EmailSubscription");

        var client = new ServiceBusClient(serviceBusConnectionString);
        _orderUpdatePaymentStatusProcessor = client.CreateProcessor(orderUpdatePaymentResultTopic, emailSubscription);
    }

    public async Task Start()
    {
        _orderUpdatePaymentStatusProcessor.ProcessMessageAsync += OnOrderPaymentUpdateReceived;
        _orderUpdatePaymentStatusProcessor.ProcessErrorAsync += ErrorHandler;
        await _orderUpdatePaymentStatusProcessor.StartProcessingAsync();
    }
    
    public async Task Stop()
    {
        await _orderUpdatePaymentStatusProcessor.StopProcessingAsync();
        _orderUpdatePaymentStatusProcessor.ProcessMessageAsync -= OnOrderPaymentUpdateReceived;
        _orderUpdatePaymentStatusProcessor.ProcessErrorAsync -= ErrorHandler;
        await _orderUpdatePaymentStatusProcessor.DisposeAsync();
    }

    private async Task OnOrderPaymentUpdateReceived(ProcessMessageEventArgs args)
    {
        var message = args.Message;
        var body = Encoding.UTF8.GetString(message.Body);
        var updatePaymentResultMessage = JsonConvert.DeserializeObject<UpdatePaymentResultMessage>(body);

        await _emailRepository.SendAndLogEmail(updatePaymentResultMessage);
        await args.CompleteMessageAsync(args.Message);
    }

    private async Task ErrorHandler(ProcessErrorEventArgs arg)
    {
        Console.WriteLine(arg.Exception.ToString());
        await Task.CompletedTask;
    }
}