using API.Extensions;
using API.Middleware;
using API.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using API.Entites;
using API.Entities;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddApplicationServices(builder.Configuration); // Add general application services
builder.Services.AddIdentityServices(builder.Configuration); // Add authentication/identity services

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// Add ExceptionMiddleware to the pipeline
app.UseMiddleware<ExceptionMiddleware>(); // Global exception handling middleware

app.UseHttpsRedirection();

app.UseCors(x => x.AllowAnyHeader()
                  .AllowAnyMethod()
                  .WithOrigins("https://localhost:4200")); // Allow Angular frontend

app.UseCors("AllowFrontend");

app.UseAuthentication(); // Enable authentication
app.UseAuthorization();  // Enable authorization

// Seed database
using var scope = app.Services.CreateScope();
var services = scope.ServiceProvider;

try
{
    var context = services.GetRequiredService<DataContext>();
    var userManager = services.GetRequiredService<UserManager<AppUser>>();
    var roleManager = services.GetRequiredService<RoleManager<AppRole>>();
    await context.Database.MigrateAsync(); // Apply migrations
    await Seed.SeedUsers(userManager, roleManager); // Seed the database
}
catch (Exception ex)
{
    var logger = services.GetRequiredService<ILogger<Program>>();
    logger.LogError(ex, "An error occurred during migration or seeding.");
}

app.MapControllers();

app.Run();
