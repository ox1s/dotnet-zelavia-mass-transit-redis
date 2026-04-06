var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();
builder.Services.AddProblemDetails();
builder.Services.AddOpenApi();
var app = builder.Build();
app.UseExceptionHandler();
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

List<Arrival> arrivals = [new Arrival(Guid.NewGuid(), DateTime.Now.AddDays(2), 100), new Arrival(Guid.NewGuid(), DateTime.Now.AddDays(3), 10), new Arrival(Guid.NewGuid(), DateTime.Now.AddDays(4), 1000)];

app.MapGet("/arrivals", () =>
{
    return arrivals;
});

app.MapDefaultEndpoints();

app.Run();

record Arrival(Guid Id, DateTime ArrivalUtc, decimal price);