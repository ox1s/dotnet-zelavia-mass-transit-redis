using MassTransit;
using Microsoft.EntityFrameworkCore;
using MongoDB.Driver;
using zelavia.PaymentsApi;
using zelavia.PaymentsApi.Consumers;
using zelavia.PaymentsApi.Data;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddOpenApi();

builder.Services.AddTransient<IPaymentService, PaymentService>();

builder.AddMongoDBClient("mongo");

builder.Services.AddScoped<PaymentDbContext>(sp =>
{
    var client = sp.GetRequiredService<IMongoClient>();

    var database = client.GetDatabase("payments");
    return PaymentDbContext.Create(database);
});

builder.Services.AddMassTransit(x =>
{
    x.AddConsumer<ProcessPaymentConsumer>();

    x.UsingRabbitMq((context, cfg) =>
    {
        cfg.Host(builder.Configuration.GetConnectionString("messaging"));
        cfg.ConfigureEndpoints(context);
    });
});

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.Run();
