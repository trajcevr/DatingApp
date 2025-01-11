using System;
using System.Net;
using System.Text.Json;
using System.Threading.Tasks;
using API.Error;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace API.Middleware
{
    public class ExceptionMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ExceptionMiddleware> _logger;
        private readonly IHostEnvironment _env;

        public ExceptionMiddleware(RequestDelegate next, ILogger<ExceptionMiddleware> logger, IHostEnvironment env)
        {
            _next = next;
            _logger = logger;
            _env = env;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context); // Proceed with the next middleware in the pipeline
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An unexpected error occurred."); // Log the exception

                context.Response.ContentType = "application/json"; // Set the response content type
                context.Response.StatusCode = (int)HttpStatusCode.InternalServerError; // Set HTTP status code

                // Prepare the response object
                var response = _env.IsDevelopment()
                    ? new ApiException(context.Response.StatusCode, ex.Message, ex.StackTrace?.ToString())
                    : new ApiException(context.Response.StatusCode, "Internal Server Error");

                var options = new JsonSerializerOptions
                {
                    PropertyNamingPolicy = JsonNamingPolicy.CamelCase // Use camel case for JSON keys
                };

                var json = JsonSerializer.Serialize(response, options); // Serialize the response object

                await context.Response.WriteAsync(json); // Write the JSON response
            }
        }
    }
}
