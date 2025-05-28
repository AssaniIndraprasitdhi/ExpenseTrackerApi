using Microsoft.AspNetCore.Http;
using System;
using System.Net;
using System.Text.Json;
using System.Threading.Tasks;

namespace ExpenseTrackerApi.Middleware
{
    public class ErrorHandlingMiddleware
    {
        private readonly RequestDelegate _next;
        public ErrorHandlingMiddleware(RequestDelegate next) => _next = next;

        public async Task Invoke(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception ex)
            {
                context.Response.ContentType = "application/json";
                context.Response.StatusCode  = (int)HttpStatusCode.InternalServerError;

                var result = JsonSerializer.Serialize(new
                {
                    message = ex.Message,
                });

                await context.Response.WriteAsync(result);
            }
        }
    }
}
