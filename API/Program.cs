using API.Extensions;
using API.Middleware;

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

app.UseAuthentication(); // Enable authentication
app.UseAuthorization();  // Enable authorization

app.MapControllers();

app.Run();
