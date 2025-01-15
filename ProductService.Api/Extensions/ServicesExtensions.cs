using MassTransit;
using Microsoft.Extensions.Options;
using ProductService.Application.Contracts;
using ProductService.Application.EventBus;
using ProductService.Application.Repositories;
using ProductService.Application.Services;
using ProductService.Infrastructure.MessageBroker;
using ProductService.Infrastructure.Repositories;
using ProductService.Infrastructure.Services;

namespace ProductService.Api.Extensions;
internal static class ServicesExtensions
{
    public static void AddProductServiceServices(this IServiceCollection services)
    {
        services.AddScoped<IProductRepository, ProductRepository>();
        services.AddScoped<IPriceHistoryRepository, PriceHistoryRepository>();
    }

    public static IServiceCollection AddExchangeRateApi(this IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<ExchangeRateApiSettings>(configuration.GetSection("ExchangeRateApi"));
        services.AddSingleton(sp =>
            sp.GetRequiredService<IOptions<ExchangeRateApiSettings>>().Value);
        services.AddHttpClient<ExchangeRateApiService>((provider, client) =>
        {
            var settings = provider.GetRequiredService<IOptions<ExchangeRateApiSettings>>().Value;
            client.BaseAddress = new Uri($"{settings.BaseUrl}/{settings.ApiKey}/");
        });

        services.AddTransient<IExchangeRateApiService, ExchangeRateApiService>();
        return services;
    }

    public static IServiceCollection AddRabbitMQ(this IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<RabbitMQSettings>(configuration.GetSection("RabbitMQ"));
        services.AddSingleton(sp =>
            sp.GetRequiredService<IOptions<RabbitMQSettings>>().Value);
        services.AddMassTransit(busConfigurator =>
        {
            busConfigurator.SetKebabCaseEndpointNameFormatter();
            busConfigurator.UsingRabbitMq((context, cfg) =>
            {
                var rabbitMQSettings = context.GetRequiredService<IOptions<RabbitMQSettings>>().Value;
                cfg.Host(new Uri($"amqp://{rabbitMQSettings.HostName}:{rabbitMQSettings.Port}"), h =>
                {
                    h.Username(rabbitMQSettings.UserName);
                    h.Password(rabbitMQSettings.Password);
                });
            });
        });

        services.AddTransient<IEventBus, EventBus>();
        
        return services;
    }
}