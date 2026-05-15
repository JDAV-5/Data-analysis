namespace ETLService.Middleware
{
    public class RequestTrackingMiddleware
    {
        private readonly RequestDelegate _next;

        public RequestTrackingMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task Invoke(HttpContext context)
        {
            var user = context.User.Identity?.Name ?? "Anonimo";

            var endpoint = context.Request.Path;

            var method = context.Request.Method;

            var ip = context.Connection.RemoteIpAddress?.ToString();

            var date = DateTime.Now;

            Console.WriteLine(
                $"[{date}] USER: {user} | METHOD: {method} | ENDPOINT: {endpoint} | IP: {ip}"
            );

            await _next(context);
        }
    }
}