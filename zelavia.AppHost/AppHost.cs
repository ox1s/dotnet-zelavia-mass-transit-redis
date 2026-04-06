var builder = DistributedApplication.CreateBuilder(args);

var postgres = builder.AddPostgres("postgres");
var postgresdb = postgres.AddDatabase("postgresdb");
var rabbitmq = builder.AddRabbitMQ("messaging")
     .WithManagementPlugin();
var mongo = builder.AddMongoDB("mongo")
                   .WithLifetime(ContainerLifetime.Persistent);
var mongodb = mongo.AddDatabase("payments");

var usersApi = builder.AddProject<Projects.zelavia_UsersApi>("users-api")
    .WithHttpHealthCheck("/health");

var flightbookings = builder.AddProject<Projects.zelavia_FlightBookingApi>("flightbookings-api")
    .WithHttpHealthCheck("/health")
    .WaitFor(rabbitmq)
    .WithReference(rabbitmq)
    .WaitFor(postgresdb)
    .WithReference(postgresdb);
var paymentsApi = builder.AddProject<Projects.zelavia_PaymentsApi>("payments-api")
    .WithHttpHealthCheck("/health")
    .WaitFor(rabbitmq)
    .WithReference(rabbitmq)
    .WaitFor(mongo)
    .WithReference(mongo)
    .WaitFor(mongodb)
    .WithReference(mongodb);



builder.AddProject<Projects.zelavia_Web>("webfrontend")
    .WithExternalHttpEndpoints()
    .WithHttpHealthCheck("/health")
    .WithReference(usersApi)
    .WaitFor(usersApi)
    .WithReference(flightbookings)
    .WaitFor(flightbookings);

builder.Build().Run();
