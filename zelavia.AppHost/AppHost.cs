using k8s.KubeConfigModels;

var builder = DistributedApplication.CreateBuilder(args);

var postgres = builder.AddPostgres("postgres");
var postgresdb = postgres.AddDatabase("postgresdb");
var rabbitmq = builder.AddRabbitMQ("messaging")
     .WithManagementPlugin();
var mongo = builder.AddMongoDB("mongo")
                   .WithLifetime(ContainerLifetime.Persistent);
var mongodb = mongo.AddDatabase("payments");
var mailpit = builder.AddMailPit("mailpit");

var usersApi = builder.AddProject<Projects.zelavia_UsersApi>("users-api")
    .WithHttpHealthCheck("/health");

var flightbookings = builder.AddProject<Projects.zelavia_FlightBookingApi>("flightbookings-api")
    .WithHttpHealthCheck("/health")
    .WaitFor(rabbitmq)
    .WithReference(rabbitmq)
    .WaitFor(postgresdb)
    .WithReference(postgresdb);
var paymentsApi = builder.AddProject<Projects.zelavia_PaymentsApi>("payments-api")
    .WithExternalHttpEndpoints()
    .WithReference(usersApi)
    .WaitFor(usersApi)
    .WithHttpHealthCheck("/health")
    .WaitFor(rabbitmq)
    .WithReference(rabbitmq)
    .WaitFor(mongo)
    .WithReference(mongo)
    .WaitFor(mongodb)
    .WithReference(mongodb);

var ticketingApi = builder.AddProject<Projects.zelavia_TicketingApi>("ticketing-api")
    .WithHttpHealthCheck("/health")
    .WaitFor(rabbitmq)
    .WithReference(rabbitmq)
    .WithReference(mailpit);


builder.AddProject<Projects.zelavia_Web>("webfrontend")
    .WithExternalHttpEndpoints()
    .WithHttpHealthCheck("/health")
    .WithReference(usersApi)
    .WaitFor(usersApi)
    .WithReference(flightbookings)
    .WaitFor(flightbookings);

//builder.AddViteApp("client-app", "../../zelavia.WebClient")
//    .WithReference(usersApi)
//    .WithReference(flightbookings)
//    .WithEnvironment("VITE_API_FLIGHT_URL", flightbookings.GetEndpoint("https"))
//    .WithEnvironment("VITE_API_USER_URL", usersApi.GetEndpoint("https"))

//    .WithViteConfig("./vite.config.js")

//    .WaitFor(usersApi)
//    .WaitFor(flightbookings);

builder.Build().Run();
