using BuildingBlocks.Observability.ApiClient;
using BuildingBlocks.Observability.Correlation;
using BuildingBlocks.Observability.ExceptionHandler;
using BuildingBlocks.Observability.Logging;
using Orders.Api.Endpoints;


var builder = WebApplication.CreateBuilder(args);
builder.Host.AddLogging();
// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();
builder.Services.AddSwaggerGen();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddExceptionHandler<GlobalExceptionHandler>();
builder.Services.AddProblemDetails();
builder.Services.AddHttpClient("TelemetryClient", client =>
{
    client.BaseAddress = new Uri(builder.Configuration["BaseUrl:Url"]!);
    client.Timeout = TimeSpan.FromSeconds(5);
});
builder.Services.AddSingleton<TelemetryClient>();
builder.Services.AddHealthChecks();


var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
app.UseCorrelationId();
app.UseExceptionHandler();
app.UseObservabilityRequestLogging();
app.UseHttpsRedirection();
app.UseRouting();
app.MapOrderEndpoints(); 


app.Run();

