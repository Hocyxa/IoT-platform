using IoT.ControlPlane.Application;
using IoT.ControlPlane.Infrastructure;
using IoT.ControlPlane.Infrastructure.Persistence;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

// Register application services.
builder.Services.AddOpenApi();
builder.Services.AddInfrastructure(builder.Configuration);
builder.Services.AddApplication();

WebApplication app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider
        .GetRequiredService<ControlPlaneDbContext>();

    var canConnect = await dbContext.Database.CanConnectAsync();

    Console.WriteLine($"PostgreSQL connection: {canConnect}");
}


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
