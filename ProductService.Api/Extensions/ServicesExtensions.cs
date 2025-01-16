using System.Text;
using MassTransit;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
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
                cfg.ConfigureEndpoints(context);
            });
        });

        services.AddTransient<IEventBus, EventBus>();

        return services;
    }

    public static IServiceCollection AddAuth(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(options =>
            {
                var secret = Encoding.UTF8.GetBytes(configuration["Auth:Secret"]!);
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = configuration["Auth:Issuer"],
                    ValidAudience = configuration["Auth:Audience"],
                    IssuerSigningKey = new SymmetricSecurityKey(secret)
                };
                options.Authority = configuration["Auth:Issuer"];
                options.RequireHttpsMetadata = true;
            });

        return services;
    }

    public static IServiceCollection AddConfigSettings(this IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<AuthSettings>(configuration.GetSection("Auth"));
        services.AddSingleton(sp =>
            sp.GetRequiredService<IOptions<AuthSettings>>().Value);
        return services;
    }
}