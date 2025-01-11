using ProductService.Application.Repositories;
using ProductService.Infrastructure.Repositories;

namespace ProductService.Api.Extensions;
internal static class ServicesExtensions
{
    public static void AddProductServiceServices(this IServiceCollection services)
    {
        services.AddScoped<IProductRepository, ProductRepository>();
        services.AddScoped<IPriceHistoryRepository, PriceHistoryRepository>();
    }
}