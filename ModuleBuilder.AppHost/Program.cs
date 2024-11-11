var builder = DistributedApplication.CreateBuilder(args);

var cache = builder.AddRedis("cache");
var cosmos = builder.AddAzureCosmosDB("graphDb");
var cosmosdb = cosmos.AddDatabase("modules")
    .RunAsEmulator(builder => 
        builder.WithLifetime(ContainerLifetime.Persistent)
    );

var apiService = builder.AddProject<Projects.ModuleBuilder_ApiService>("apiservice")
    .WithReference(cosmosdb)
    .WaitFor(cosmosdb);

builder.AddProject<Projects.ModuleBuilder_Web>("webfrontend")
    .WithExternalHttpEndpoints()
    .WithReference(cache)
    .WaitFor(cache)
    .WithReference(apiService)
    .WaitFor(apiService);

builder.Build().Run();
