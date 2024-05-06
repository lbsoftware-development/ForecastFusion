using System.Net;

namespace ForecastFusion.Api.Middleware
{
    public class GlobalExceptionHandlingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly Serilog.ILogger _logger;

        public GlobalExceptionHandlingMiddleware(Serilog.ILogger logger, RequestDelegate next)
        {
            _logger = logger;
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception e)
            {
                _logger.Error(e, e.Message);
                context.Response.StatusCode =(int)HttpStatusCode.InternalServerError;
                throw;
            }            
        }
    }
}
