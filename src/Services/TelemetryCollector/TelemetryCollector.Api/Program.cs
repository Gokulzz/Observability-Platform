using BuildingBlocks.Observability.Correlation;
using BuildingBlocks.Observability.ExceptionHandler;
using BuildingBlocks.Observability.Logging;
using TelemetryCollector.Api.Configurations;
using TelemetryCollector.Api.Endpoints;

var builder = WebApplication.CreateBuilder(args);
// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Host.AddLogging();
builder.Services.AddOpenApi();
builder.Services.AddInfrastructureServices(builder.Configuration);  
builder.Services.AddApplicationServices();
builder.Services.AddSwaggerGen();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddProblemDetails();
builder.Services.AddExceptionHandler<GlobalExceptionHandler>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
app.UseExceptionHandler();
app.UseHttpsRedirection();
app.UseCorrelationId();
app.UseObservabilityRequestLogging();
app.UseRouting();
app.MapTelemetryEndpoints();
app.MapSlaPolicyEndpoints();    
app.Run();

