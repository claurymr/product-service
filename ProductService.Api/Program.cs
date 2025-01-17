using FastEndpoints;
using FluentValidation;
using Microsoft.EntityFrameworkCore;
using ProductService.Api.Extensions;
using ProductService.Api.Middlewares;
using ProductService.Application.Validation.Validators;
using ProductService.Infrastructure;
using ProductService.Infrastructure.Data;
using ProductService.Infrastructure.Extensions;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddConfigSettings(builder.Configuration);
builder.Services.AddDbContext<ProductServiceDbContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("ProductServiceConnection")));
builder.Services.AddAuth(builder.Configuration);
builder.Services.AddAuthorizationBuilder()
    .AddPolicy("AdminOnly", policy => policy.RequireRole("admin"))
    .AddPolicy("AdminOrUser", policy => policy.RequireRole("admin", "user"));
builder.Services.AddValidatorsFromAssemblyContaining<CreateProductCommandValidator>();
builder.Services.AddMediatR(config =>
    config.RegisterServicesFromAssembly(typeof(DesignTimeContextFactory).Assembly));
builder.Services.AddRabbitMQ(builder.Configuration);
builder.Services.AddProductServiceServices();
builder.Services.AddExchangeRateApi(builder.Configuration);
builder.Services.AddFastEndpoints();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

if (!app.Environment.IsDevelopment())
{
    app.UseHttpsRedirection();
}

app.UseRouting();
app.UseMiddleware<ErrorHandlerMiddleware>();
app.UseAuthentication();
app.UseAuthorization();
app.UseFastEndpoints();
app.MigrateDatabase();
app.Run();
