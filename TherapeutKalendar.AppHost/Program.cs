var builder = DistributedApplication.CreateBuilder(args);

var postgres = builder.AddPostgres("postgres")
    .WithDataVolume()
    .WithPgAdmin();
var initscript = File.ReadAllText("db/init.sql");
var postgresdb = postgres.AddDatabase("termindb")
        .WithCreationScript(initscript);

var authService = builder.AddProject<Projects.AuthService>("auth-service")
    .WithReference(postgresdb)
    .WithEnvironment("Jwt__Key", builder.Configuration["Jwt:Key"]);

var terminService = builder.AddProject<Projects.TerminService>("termin-service")
    .WithReference(postgresdb)
    .WithReference(authService);

builder.Build().Run();
