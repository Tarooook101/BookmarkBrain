

namespace BookMarkBrain.API.Extensions;

public static class ApiServiceExtensions
{
    public static IServiceCollection AddAPIServices(this IServiceCollection services, IConfiguration configuration)
    {
        // Configure CORS
        services.AddCors(opt =>
        {
            opt.AddPolicy("CorsPolicy", policy =>
            {
                policy.AllowAnyHeader()
                      .AllowAnyMethod()
                      .WithOrigins(configuration.GetSection("AllowedOrigins").Get<string[]>() ?? new[] { "https://localhost:5001" });
            });
        });

        return services;
    }
}