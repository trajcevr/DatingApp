// using System;
// using Microsoft.EntityFrameworkCore;


// var builder = WebApplication.CreateBuilder(args);

// // Add services to the container.

// builder.Services.AddControllers();
// // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
// builder.Services.AddEndpointsApiExplorer();
// builder.Services.AddSwaggerGen();

// builder.Services.AddDbContext<API.Data.DataContext>(options =>
//     options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection")));


// var app = builder.Build();

// // Configure the HTTP request pipeline.
// if (app.Environment.IsDevelopment())
// {
//     app.UseSwagger();
//     app.UseSwaggerUI();
// }

// app.UseHttpsRedirection();

// app.UseCors(x => x.AllowAnyHeader().AllowAnyMethod().WithOrigins("http//localhost:4200"));

// app.UseAuthorization();

// app.MapControllers();

// app.Run();
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Add the DbContext for your application, connecting to SQLite.
builder.Services.AddDbContext<API.Data.DataContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection"))
);

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// Enable HTTPS redirection.
app.UseHttpsRedirection();

// CORS Configuration
app.UseCors(x => x.AllowAnyHeader()
                  .AllowAnyMethod()
                  .WithOrigins("https://localhost:4200"));  // Allow Angular frontend

// Enable Authorization (if you are using authentication and authorization)
app.UseAuthorization();

// Map controllers to handle HTTP requests
app.MapControllers();

// Run the application
app.Run();
