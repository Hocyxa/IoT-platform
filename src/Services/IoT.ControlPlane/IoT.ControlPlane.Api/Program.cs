
WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

// Register application services.
builder.Services.AddOpenApi();

WebApplication app = builder.Build();

// Expose OpenAPI specification in development.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.MapGet("/health", () => Results.Ok(new
{
    Status = "Healthy",
    Service = "IoT.ControlPlane"
}))
.WithName("HealthCheck");

app.Run();
