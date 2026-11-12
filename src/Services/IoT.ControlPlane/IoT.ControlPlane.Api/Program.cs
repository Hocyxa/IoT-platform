using Asp.Versioning;

using IoT.ControlPlane.Application;
using IoT.ControlPlane.Infrastructure;
using IoT.ControlPlane.Infrastructure.Persistence;

using Microsoft.EntityFrameworkCore;
WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

// Register application services.
builder.Services.AddOpenApi();
builder.Services.AddInfrastructure(builder.Configuration);
builder.Services.AddApplication();

builder.Services.AddControllers();
builder.Services.AddSwaggerGen();

builder.Services
    .AddApiVersioning(options =>
    {
        options.DefaultApiVersion = new ApiVersion(1, 0);
        options.AssumeDefaultVersionWhenUnspecified = true;
        options.ReportApiVersions = true;
        options.ApiVersionReader = new QueryStringApiVersionReader("v");

    })
    .AddMvc();
builder.Services.Configure<RouteOptions>(options =>
{
    options.LowercaseUrls = true; // Делает URL строчными
    options.LowercaseQueryStrings = true; // (Опционально) Делает параметры запроса строчными
});

WebApplication app = builder.Build();

// Expose OpenAPI specification in development.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();

    app.UseSwagger();
    app.UseSwaggerUI();


    using (var scope = app.Services.CreateScope())
    {
        var dbContext = scope.ServiceProvider
            .GetRequiredService<ControlPlaneDbContext>();
        var canConnect = await dbContext.Database.CanConnectAsync();
        Console.WriteLine($"PostgreSQL connection: {canConnect}");
    }

}
if (app.Environment.IsEnvironment("Testing"))
{
    using (var scope = app.Services.CreateScope())
    {
        var dbContext = scope.ServiceProvider
            .GetRequiredService<ControlPlaneDbContext>();

        await dbContext.Database.MigrateAsync();
    }
}

if (!app.Environment.IsEnvironment("Testing"))
{
    app.UseHttpsRedirection();
}
app.MapControllers();
app.MapGet("/health", () => Results.Ok(new
{
    Status = "Healthy",
    Service = "IoT.ControlPlane"
}))
.WithName("HealthCheck");

app.Run();
