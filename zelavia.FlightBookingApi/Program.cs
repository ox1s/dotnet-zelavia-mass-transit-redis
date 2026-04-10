using MassTransit;
using Microsoft.EntityFrameworkCore;
using zelavia.Contracts.Messages;
using zelavia.FlightBookingApi.Data;
using zelavia.FlightBookingApi.Sagas;

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
        })
        .Endpoint(e =>
        {
            e.Name = "booking-state";
            e.ConcurrentMessageLimit = 8;
        });

    x.UsingRabbitMq((context, cfg) =>
    {
        cfg.Host(builder.Configuration.GetConnectionString("messaging"));
        cfg.ConfigureEndpoints(context);
    });
});

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<BookingDbContext>();
    dbContext.Database.Migrate();
}


app.UseExceptionHandler();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

var flights = new List<Flight>();

app.MapGet("/flights", () => flights);

app.MapPost("/flights/{flightId:guid}/book", async (
    Guid flightId,
    BookFlightRequest request,
    IPublishEndpoint endpoint) =>
{
    var flight = flights.FirstOrDefault(x => x.Id == flightId);

    if (flight is null)
    {
        return Results.NotFound();
    }

    await endpoint.Publish(new BookingCreated {
        BookingDateUtc = DateTime.UtcNow,
        BookingId = Guid.NewGuid(),
        UserId = request.UserId,
        UserEmail = request.UserEmail,
        Amount = flight.Price
    });

    return Results.Created();
});

app.MapPost("/flights/generate", () =>
{
    for (int i = 0; i < 5; i++)
    {
        flights.Add(new Flight(Guid.NewGuid(), DateTime.UtcNow, (decimal)Math.Pow(10, i)));
    }
});

app.MapDefaultEndpoints();

app.Run();

public record Flight(Guid Id, DateTime FlightUtc, decimal Price);
record Booking(Guid ArrivalId, Guid UserId, DateTime BookUtc);
public record BookFlightRequest(Guid UserId, string UserEmail);