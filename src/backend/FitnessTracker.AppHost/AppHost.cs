var builder = DistributedApplication.CreateBuilder(args);

var postgres = builder.AddPostgres("postgres", port: 51547)
    .AddDatabase("fitness-tracker");

var rabbitmq = builder.AddRabbitMQ("rabbitmq")
    .WithManagementPlugin();

var redis = builder.AddRedis("redis");

var migrator = builder.AddProject<Projects.FitnessTracker_Migrator>("migrator")
    .WithReference(postgres)
    .WaitFor(postgres);

var api = builder.AddProject<Projects.FitnessTracker_Api>("api")
    .WithEndpoint("https", e => e.Port = 7269)
    .WithReference(postgres)
    .WithReference(rabbitmq)
    .WithReference(redis)
    .WaitFor(postgres)
    .WaitFor(rabbitmq)
    .WaitFor(redis)
    .WaitFor(migrator);

builder.Build().Run();
