using OpenTelemetry.Logs;
using OpenTelemetry.Metrics;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;
using server.Services;

var builder = WebApplication.CreateBuilder(args);

// Enable otel experimental grpc instrumentation: https://github.com/open-telemetry/opentelemetry-dotnet/blob/main/src/OpenTelemetry.Instrumentation.AspNetCore/README.md#experimental-support-for-grpc-requests
builder.Configuration.AddInMemoryCollection(
  new Dictionary<string, string?>
  {
    ["OTEL_DOTNET_EXPERIMENTAL_ASPNETCORE_ENABLE_GRPC_INSTRUMENTATION"] = "true",
  });

var otel = builder.Services.AddOpenTelemetry();

otel.ConfigureResource(resource => resource
  .AddService("dotnet-server"));

otel.WithMetrics(metrics => metrics
  .AddAspNetCoreInstrumentation()
  .AddMeter("Microsoft.AspNetCore.Hosting")
  .AddMeter("Microsoft.AspNetCore.Server.Kestrel")
  .AddPrometheusExporter());

otel.WithTracing(tracing =>
{
  tracing.AddAspNetCoreInstrumentation();
  tracing.AddHttpClientInstrumentation();
  tracing.AddSource("server.Services.GenericService");
  tracing.AddOtlpExporter();
});

// Add services to the container.
builder.Services.AddGrpc();
builder.Services.AddGrpcReflection();

var app = builder.Build();

// Configure the HTTP request pipeline.
app.MapGrpcService<GenericService>();
app.MapGet("/", () => "Communication with gRPC endpoints must be made through a gRPC client. To learn how to create a client, visit: https://go.microsoft.com/fwlink/?linkid=2086909");
app.MapPrometheusScrapingEndpoint();

// Enable gRPC reflection
app.MapGrpcReflectionService();

app.Run();
