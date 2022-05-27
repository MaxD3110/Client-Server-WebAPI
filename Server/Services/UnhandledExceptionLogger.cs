using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;

namespace Server.Services
{
    public class UnhandledExceptionLogger : IMiddleware
    {
        private readonly ILogger<UnhandledExceptionLogger> _logger;

        public UnhandledExceptionLogger(ILogger<UnhandledExceptionLogger> logger)
        {
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context, RequestDelegate next)
        {
            try
            {
                await next(context);
            }
            catch (Exception exception)
            {
                const string message = "An unhandled exception has occurred while executing the request.";
                _logger.LogError(exception, message);

                context.Response.Clear();
                context.Response.StatusCode = StatusCodes.Status500InternalServerError;
            }
        }
    }
}