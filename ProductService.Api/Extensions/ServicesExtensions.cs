using Microsoft.Extensions.Options;
using ProductService.Application.Contracts;
using ProductService.Application.Repositories;
using ProductService.Application.Services;
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
        services.AddHttpClient<ExchangeRateApiService>((provider, client) =>
        {
            var settings = provider.GetRequiredService<IOptions<ExchangeRateApiSettings>>().Value;
            client.BaseAddress = new Uri($"{settings.BaseUrl}/{settings.ApiKey}/");
        });

        services.AddTransient<IExchangeRateApiService, ExchangeRateApiService>();

        return services;
    }
}