var builder = DistributedApplication.CreateBuilder(args);

var usersApi = builder.AddProject<Projects.zelavia_UsersApi>("users-api")
    .WithHttpHealthCheck("/health");

var bookingApi = builder.AddProject<Projects.zelavia_BookingsApi>("bookings-api")
    .WithHttpHealthCheck("/health");

builder.AddProject<Projects.zelavia_Web>("webfrontend")
    .WithExternalHttpEndpoints()
    .WithHttpHealthCheck("/health")
    .WithReference(usersApi)
    .WaitFor(usersApi)
    .WithReference(bookingApi)
    .WaitFor(bookingApi);

builder.Build().Run();
