using OpenTelemetry.Logs;
using OpenTelemetry.Metrics;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;
using server.Services;

var builder = WebApplication.CreateBuilder(args);

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
  tracing.AddOtlpExporter(opt =>
  {
    opt.Endpoint = new Uri("localhost:4317");
  });
});

// Add services to the container.
builder.Services.AddGrpc();
builder.Services.AddGrpcReflection();

var app = builder.Build();

// Configure the HTTP request pipeline.
app.MapGrpcService<GenericService>();
app.MapGet("/", () => "Communication with gRPC endpoints must be made through a gRPC client. To learn how to create a client, visit: https://go.microsoft.com/fwlink/?linkid=2086909");
app.MapPrometheusScrapingEndpoint("localhost:8080/metrics");

// Enable gRPC reflection
app.MapGrpcReflectionService();

app.Run();
