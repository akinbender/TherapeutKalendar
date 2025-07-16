using Npgsql;
using TerminService.Data;

var builder = WebApplication.CreateBuilder(args);
builder.AddServiceDefaults();
builder.Services.AddGrpc(options =>
{
    options.EnableDetailedErrors = true; // Only in development
    //options.Interceptors.Add<AuthInterceptor>(); // If using auth
});

builder.Services.AddCors(o => o.AddPolicy("AllowAll", builder =>
{
    builder.AllowAnyOrigin()
            .AllowAnyMethod()
            .AllowAnyHeader()
            .WithExposedHeaders("Grpc-Status", "Grpc-Message", "Grpc-Encoding", "Grpc-Accept-Encoding");
}));

builder.Services.AddGrpcReflection();
// Database context (example for Dapper)
builder.Services.AddScoped(_ => new NpgsqlConnection(
    builder.Configuration.GetConnectionString("postgres")));
builder.Services.AddScoped<ITerminRepository, TerminRepository>();
builder.Services.AddScoped<ITherapistRepository, TherapistRepository>();
var app = builder.Build();


app.UseGrpcWeb(new GrpcWebOptions { DefaultEnabled = true });
app.UseCors("AllowAll");
app.MapGrpcService<TerminService.Services.TerminService>().EnableGrpcWeb().RequireCors("AllowAll");
app.MapGrpcService<TerminService.Services.TherapistService>().EnableGrpcWeb().RequireCors("AllowAll");
app.MapGrpcReflectionService(); // For gRPC reflection, useful for debugging
app.MapGet("/", () => "gRPC Server is running");

app.Run();