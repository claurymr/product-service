using System.Security.Claims;
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

/// <summary>
/// Provides extension methods for adding services to the IServiceCollection.
/// </summary>
internal static class ServicesExtensions
{
    /// <summary>
    /// Adds the product service related services to the IServiceCollection.
    /// </summary>
    /// <param name="services">The IServiceCollection to add services to.</param>
    public static void AddProductServiceServices(this IServiceCollection services)
    {
        services.AddScoped<IProductRepository, ProductRepository>();
        services.AddScoped<IPriceHistoryRepository, PriceHistoryRepository>();
    }

    /// <summary>
    /// Adds the ExchangeRateApi service to the IServiceCollection.
    /// </summary>
    /// <param name="services">The IServiceCollection to add services to.</param>
    /// <param name="configuration">The IConfiguration to retrieve settings from.</param>
    /// <returns>The updated IServiceCollection.</returns>
    /// <exception cref="InvalidOperationException">Thrown when the ExchangeRateApi BaseUrl is not configured.</exception>
    public static IServiceCollection AddExchangeRateApi(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddHttpClient<IExchangeRateApiService, ExchangeRateApiService>((provider, client) =>
        {
            var baseUrl = configuration["ExchangeRateApi:BaseUrl"];
            var logger = provider.GetRequiredService<ILogger<ExchangeRateApiSettings>>();
            logger.LogInformation("Setting up ExchangeRateApiService with {BaseAddress}", baseUrl);

            if (string.IsNullOrWhiteSpace(baseUrl))
            {
                throw new InvalidOperationException("ExchangeRateApi BaseUrl is not configured.");
            }

            client.BaseAddress = new Uri($"{baseUrl}/");
            client.DefaultRequestHeaders.Add("Accept", "application/json");
        });

        return services;
    }

    /// <summary>
    /// Adds RabbitMQ services to the IServiceCollection.
    /// </summary>
    /// <param name="services">The IServiceCollection to add services to.</param>
    /// <param name="configuration">The IConfiguration to retrieve settings from.</param>
    /// <returns>The updated IServiceCollection.</returns>
    public static IServiceCollection AddRabbitMQ(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddMassTransit(busConfigurator =>
        {
            busConfigurator.SetKebabCaseEndpointNameFormatter();
            busConfigurator.UsingRabbitMq((context, cfg) =>
            {
                var rabbitMQSettings = context.GetRequiredService<IOptions<RabbitMQSettings>>().Value;
                var logger = context.GetRequiredService<ILogger<RabbitMQSettings>>();
                var uri = $"amqp://{rabbitMQSettings.HostName}/";
                logger
                    .LogInformation("Connecting to RabbitMQ at {rabbitMQHostName}:{rabbitMQPort}",
                        rabbitMQSettings.HostName, rabbitMQSettings.Port);
                cfg.Host(uri, h =>
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

    /// <summary>
    /// Adds authentication and authorization services to the IServiceCollection.
    /// </summary>
    /// <param name="services">The IServiceCollection to add services to.</param>
    /// <param name="configuration">The IConfiguration to retrieve settings from.</param>
    /// <returns>The updated IServiceCollection.</returns>
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
                    IssuerSigningKey = new SymmetricSecurityKey(secret),
                    RoleClaimType = ClaimTypes.Role
                };
                options.Authority = configuration["Auth:Issuer"];
                options.RequireHttpsMetadata = false;
            });

        services.AddAuthorizationBuilder()
            .AddPolicy("AdminOnly", policy => policy.RequireRole("admin"))
            .AddPolicy("AdminOrUser", policy => policy.RequireRole("admin", "user"));

        return services;
    }

    /// <summary>
    /// Adds configuration settings to the IServiceCollection.
    /// </summary>
    /// <param name="services">The IServiceCollection to add services to.</param>
    /// <param name="configuration">The IConfiguration to retrieve settings from.</param>
    /// <returns>The updated IServiceCollection.</returns>
    public static IServiceCollection AddConfigSettings(this IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<AuthSettings>(configuration.GetSection("Auth"));
        services.AddSingleton(sp =>
            sp.GetRequiredService<IOptions<AuthSettings>>().Value);
        services.Configure<ExchangeRateApiSettings>(configuration.GetSection("ExchangeRateApi"));
        services.AddTransient(sp =>
            sp.GetRequiredService<IOptions<ExchangeRateApiSettings>>().Value);
        services.Configure<RabbitMQSettings>(configuration.GetSection("RabbitMQ"));
        services.AddSingleton(sp =>
            sp.GetRequiredService<IOptions<RabbitMQSettings>>().Value);
        return services;
    }
}