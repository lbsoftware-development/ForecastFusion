namespace ForecastFusion.Api.Middleware
{
    public class IdempotencyMiddleware
    {
        private readonly RequestDelegate _next;
        private static readonly Dictionary<string, DateTime> _processedRequests = new Dictionary<string, DateTime>();
        private static readonly TimeSpan _retentionPeriod = TimeSpan.FromSeconds(10);

        public IdempotencyMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task Invoke(HttpContext context)
        {
            string idempotencyKey = string.Empty;

            if (context.Request.Method == "PUT")
            {
                // Check if request has an idempotency key
                idempotencyKey = context.Request.Headers["Idempotency-Key"];
                if (string.IsNullOrEmpty(idempotencyKey))
                {
                    context.Response.StatusCode = 400; // Bad Request
                    await context.Response.WriteAsync("Idempotency-Key header is missing.");
                    return;
                }

                // Check if the request has already been processed
                lock (_processedRequests)
                {
                    if (_processedRequests.TryGetValue(idempotencyKey, out DateTime requestTime))
                    {
                        if (DateTime.Now - requestTime <= _retentionPeriod)
                        {
                            context.Response.StatusCode = 409; // Conflict
                            context.Response.WriteAsync("Request has already been processed.");
                            return;
                        }
                        else
                        {
                            // Remove expired entry
                            _processedRequests.Remove(idempotencyKey);
                        }
                    }
                }
            }
            // Process the request
            await _next(context);

            // Add the processed request to the dictionary
            lock (_processedRequests)
            {
                if (!string.IsNullOrEmpty(idempotencyKey))
                    _processedRequests[idempotencyKey] = DateTime.Now;
            }
        }
    }

}
