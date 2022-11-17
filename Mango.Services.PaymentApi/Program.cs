using Mango.MessageBus;
using Mango.Services.PaymentApi.Extensions;
using Mango.Services.PaymentApi.Messaging;
using Mango.Services.PaymentApi.RabbitMQSender;
using PaymentProcessor;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddSingleton<IProcessPayment, ProcessPayment>();
builder.Services.AddSingleton<IAzureServiceBusConsumer, AzureServiceBusConsumer>();

var serviceBusConnectionString = builder.Configuration["AzureServiceBus:ConnectionString"];
builder.Services.AddSingleton<IMessageBus>(new AzureServiceBusMessageBus(serviceBusConnectionString));
builder.Services.AddSingleton<IRabbitMqPaymentMessageSender, RabbitMqPaymentMessageSender>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.UseAzureServiceBusConsumer();

app.Run();