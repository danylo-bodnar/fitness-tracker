var builder = DistributedApplication.CreateBuilder(args);

var postgres = builder.AddPostgres("postgres")
    .AddDatabase("fitness-tracker");

var rabbitmq = builder.AddRabbitMQ("rabbitmq")
    .WithManagementPlugin();


var migrator = builder.AddProject<Projects.FitnessTracker_Migrator>("migrator")
    .WithReference(postgres)
    .WaitFor(postgres);

var api = builder.AddProject<Projects.FitnessTracker_Api>("api")
    .WithReference(postgres)
    .WithReference(rabbitmq)
    .WaitFor(postgres)
    .WaitFor(rabbitmq)
    .WithEnvironment("BOT_TOKEN", builder.Configuration["BOT_TOKEN"]).WaitFor(migrator);

builder.Build().Run();
