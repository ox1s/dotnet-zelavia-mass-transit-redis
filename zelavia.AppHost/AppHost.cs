var builder = DistributedApplication.CreateBuilder(args);

var usersApi = builder.AddProject<Projects.zelavia_UsersApi>("users-api")
    .WithHttpHealthCheck("/health");

var flightbookings = builder.AddProject<Projects.zelavia_FlightBookingApi>("flightbookings-api")
    .WithHttpHealthCheck("/health");

builder.AddProject<Projects.zelavia_Web>("webfrontend")
    .WithExternalHttpEndpoints()
    .WithHttpHealthCheck("/health")
    .WithReference(usersApi)
    .WaitFor(usersApi)
    .WithReference(flightbookings)
    .WaitFor(flightbookings);

builder.Build().Run();
