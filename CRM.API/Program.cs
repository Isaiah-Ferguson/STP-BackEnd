using CRM.API;
using CRM.Application;
using CRM.Infrastructure;
using CRM.Persistence;

var builder = WebApplication.CreateBuilder(args);

// ---------------------------------------------------------
// Register services from each layer via extension methods
// ---------------------------------------------------------
builder.Services.AddApplicationServices();
builder.Services.AddInfrastructureServices(builder.Configuration);
builder.Services.AddPersistenceServices(builder.Configuration);

// ---------------------------------------------------------
// Controllers
// ---------------------------------------------------------
builder.Services.AddControllers()
    .AddJsonOptions(opts =>
        opts.JsonSerializerOptions.Converters.Add(
            new System.Text.Json.Serialization.JsonStringEnumConverter()));

// ---------------------------------------------------------
// Swagger / OpenAPI
// ---------------------------------------------------------
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo
    {
        Title = "ShinyStarCRM API",
        Version = "v1",
        Description = "ASP.NET Core Web API for the ShinyStarCRM application"
    });
});

// ---------------------------------------------------------
// CORS — allow the Next.js frontend (localhost:3000)
// ---------------------------------------------------------
var frontendOrigins = builder.Configuration.GetSection("Cors:AllowedOrigins").Get<string[]>()
    ?? ["http://localhost:3000"];

builder.Services.AddCors(options =>
{
    options.AddPolicy("FrontendPolicy", policy =>
    {
        policy.WithOrigins(frontendOrigins)
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});

var app = builder.Build();

// ---------------------------------------------------------
// Middleware pipeline
// ---------------------------------------------------------
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/swagger/v1/swagger.json", "ShinyStarCRM API v1");
        options.RoutePrefix = "swagger";
    });
}

// Seed development data
if (app.Environment.IsDevelopment())
{
    using var scope = app.Services.CreateScope();
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    await DataSeeder.SeedAsync(db);
}

app.UseHttpsRedirection();
app.UseCors("FrontendPolicy");
app.UseAuthorization();
app.MapControllers();

app.Run();
