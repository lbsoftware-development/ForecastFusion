namespace ForecastFusion.Api
{
    public static class MiddlewareExtensions
    {
        public static IApplicationBuilder UseIdempotencyMiddleware(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<IdempotencyMiddleware>();
        }
    }
}
