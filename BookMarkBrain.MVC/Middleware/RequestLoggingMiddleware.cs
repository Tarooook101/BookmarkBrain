using System.Diagnostics;

namespace BookMarkBrain.MVC.Middleware;


public class RequestLoggingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<RequestLoggingMiddleware> _logger;

    public RequestLoggingMiddleware(RequestDelegate next, ILogger<RequestLoggingMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        var sw = Stopwatch.StartNew();
        var requestId = Guid.NewGuid().ToString();

        _logger.LogInformation("Request {RequestId} started: {Method} {Path}",
            requestId, context.Request.Method, context.Request.Path);

        try
        {
            await _next(context);
            sw.Stop();

            _logger.LogInformation("Request {RequestId} completed with status code {StatusCode} in {ElapsedMilliseconds}ms",
                requestId, context.Response.StatusCode, sw.ElapsedMilliseconds);
        }
        catch (Exception)
        {
            sw.Stop();
            _logger.LogWarning("Request {RequestId} failed after {ElapsedMilliseconds}ms",
                requestId, sw.ElapsedMilliseconds);
            throw;
        }
    }
}
