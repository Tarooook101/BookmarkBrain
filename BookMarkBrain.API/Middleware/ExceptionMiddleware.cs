﻿using System.Net;
using System.Text.Json;

namespace BookMarkBrain.API.Middleware;
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
            await _next(context);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An unhandled exception occurred during request processing");
            await HandleExceptionAsync(context, ex);
        }
    }

    private Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        context.Response.ContentType = "application/json";
        context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;

        var response = _env.IsDevelopment()
            ? new ApiException(context.Response.StatusCode, exception.Message, exception.StackTrace?.ToString())
            : new ApiException(context.Response.StatusCode, "Internal Server Error");

        var options = new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase };
        var json = JsonSerializer.Serialize(response, options);

        return context.Response.WriteAsync(json);
    }
}

public class ApiException
{
    public int StatusCode { get; set; }
    public string Message { get; set; }
    public string Details { get; set; }

    public ApiException(int statusCode, string message, string details = null)
    {
        StatusCode = statusCode;
        Message = message;
        Details = details;
    }
}
