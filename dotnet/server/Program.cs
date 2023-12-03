using OpenTelemetry;
using OpenTelemetry.Logs;
using OpenTelemetry.Metrics;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;
using server.Services;

var builder = WebApplication.CreateBuilder(args);



builder.Logging.AddOpenTelemetry(options =>
{
  options.SetResourceBuilder(ResourceBuilder.CreateDefault().AddService("dotnet-server")).AddOtlpExporter();
});
builder.Services.AddOpenTelemetry()
  .ConfigureResource(resource => resource.AddService("dotnet-server"))
  .WithTracing(tracing => tracing
    .AddAspNetCoreInstrumentation()
    .AddOtlpExporter()
    .AddConsoleExporter())
  .WithMetrics(metrics => metrics
    .AddAspNetCoreInstrumentation()
    .AddOtlpExporter());

// Add services to the container.
builder.Services.AddGrpc();
builder.Services.AddGrpcReflection();

var app = builder.Build();

// Configure the HTTP request pipeline.
app.MapGrpcService<GenericService>();
app.MapGet("/", () => "Communication with gRPC endpoints must be made through a gRPC client. To learn how to create a client, visit: https://go.microsoft.com/fwlink/?linkid=2086909");

// Enable gRPC reflection
app.MapGrpcReflectionService();

app.Run();
