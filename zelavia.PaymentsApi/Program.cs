using MassTransit;
using Microsoft.EntityFrameworkCore;
using MongoDB.Driver;
using zelavia.PaymentsApi;
using zelavia.PaymentsApi.Consumers;
using zelavia.PaymentsApi.Data;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddOpenApi();

builder.AddServiceDefaults();


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


builder.Services.AddHttpClient<IPaymentService, PaymentService>(client =>
{
    client.BaseAddress = new("https+http://users-api");
});

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.Run();
