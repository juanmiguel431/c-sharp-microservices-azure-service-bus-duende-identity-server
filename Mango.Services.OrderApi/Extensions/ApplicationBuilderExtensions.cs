using Mango.Services.OrderApi.Messaging;

namespace Mango.Services.OrderApi.Extensions;

public static class ApplicationBuilderExtensions
{
    private static IAzureServiceBusConsumer AzureServiceBusConsumer { get; set; }

    public static IApplicationBuilder UseAzureServiceBusConsumer(this IApplicationBuilder app)
    {
        AzureServiceBusConsumer = app.ApplicationServices.GetService<IAzureServiceBusConsumer>();
        var hostApplicationLife = app.ApplicationServices.GetService<IHostApplicationLifetime>();

        hostApplicationLife.ApplicationStarted.Register(OnStart);
        hostApplicationLife.ApplicationStopped.Register(OnStop);
        return app;
    }

    private static void OnStart()
    {
        AzureServiceBusConsumer.Start();
    }

    private static void OnStop()
    {
        AzureServiceBusConsumer.Stop();
    }
}