using MassTransit;
using Microsoft.EntityFrameworkCore;
using zelavia.BookingsApi.StateMachine;
using zelavia.FlightBookingApi.Data;

var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();
var services = builder.Services;

services.AddProblemDetails();
services.AddOpenApi();

builder.AddNpgsqlDbContext<BookingDbContext>(connectionName: "postgresdb");

services.AddMassTransit(x =>
{
    x.AddSagaStateMachine<BookingStateMachine, BookingState>()
        .EntityFrameworkRepository(r =>
        {
            r.ConcurrencyMode = ConcurrencyMode.Pessimistic;
            r.AddDbContext<DbContext, BookingDbContext>();
            r.UsePostgres();
        });

    x.UsingRabbitMq((context, cfg) =>
    {
        cfg.Host(builder.Configuration.GetConnectionString("messaging"));
        cfg.ConfigureEndpoints(context);
    });
});


var app = builder.Build();
app.UseExceptionHandler();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

List<Flight> arrivals = [new Flight(Guid.NewGuid(), DateTime.Now.AddDays(2), 100), new Flight(Guid.NewGuid(), DateTime.Now.AddDays(3), 10), new Flight(Guid.NewGuid(), DateTime.Now.AddDays(4), 1000)];

app.MapGet("/arrivals", () =>
{
    return arrivals;
});

app.MapDefaultEndpoints();

app.Run();

record Flight(Guid Id, DateTime FlightUtc, decimal price);
record Booking(Guid ArrivalId, Guid UserId, DateTime BookUtc);