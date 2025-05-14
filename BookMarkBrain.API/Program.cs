using BookMarkBrain.API.Extensions;
using BookMarkBrain.API.Middleware;
using BookMarkBrain.Core;
using BookMarkBrain.Data;
using BookMarkBrain.Data.Context;
using BookMarkBrain.Data.Seed;
using BookMarkBrain.Services;
using Microsoft.OpenApi.Models;
using Serilog;

namespace BookMarkBrain.API;
public class Program
{
    public static async Task<int> Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        // Configure Serilog
        Log.Logger = new LoggerConfiguration()
            .ReadFrom.Configuration(builder.Configuration)
            .Enrich.FromLogContext()
            .WriteTo.Console()
            .WriteTo.File("logs/bookmarkbrain-api-.log", rollingInterval: RollingInterval.Day)
            .CreateLogger();

        builder.Host.UseSerilog();

        // Add services to the container
        builder.Services.AddCoreServices();
        builder.Services.AddDataServices(builder.Configuration);
        builder.Services.AddApplicationServices(builder.Configuration);
        builder.Services.AddAPIServices(builder.Configuration);

        // Add controllers and JSON options
        builder.Services.AddControllers()
            .AddNewtonsoftJson(options =>
            {
                options.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore;
            });

        // Configure CORS with specific allowed origins
        builder.Services.AddCors(options =>
        {
            options.AddPolicy("CorsPolicy", policy =>
            {
                policy.WithOrigins(builder.Configuration.GetSection("AllowedOrigins").Get<string[]>() ??
                    new[] { "https://localhost:5001", "https://localhost:7001" })
                    .AllowAnyMethod()
                    .AllowAnyHeader();
            });
        });

        // Add basic Swagger without authentication
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen(c =>
        {
            c.SwaggerDoc("v1", new OpenApiInfo
            {
                Title = "BookMarkBrain API",
                Version = "v1",
                Description = "REST API for BookMarkBrain application"
            });
        });

        var app = builder.Build();

        // Initialize the database
        try
        {
            using (var scope = app.Services.CreateScope())
            {
                var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();

                // Apply migrations and seed data
                await DbInitializer.InitializeDatabaseAsync(app.Services);

                Log.Information("Database initialized successfully");
            }
        }
        catch (Exception ex)
        {
            Log.Error(ex, "An error occurred while initializing the database");
        }

        // Configure the HTTP request pipeline
        if (app.Environment.IsDevelopment())
        {
            app.UseDeveloperExceptionPage();
            app.UseSwagger();
            app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "BookMarkBrain API v1"));
        }
        else
        {
            app.UseHsts();
        }

        app.UseHttpsRedirection();
        app.UseRouting();

        // Custom middleware
        app.UseMiddleware<ExceptionMiddleware>();
        app.UseMiddleware<RequestLoggingMiddleware>();

        // CORS must be configured before endpoints
        app.UseCors("CorsPolicy");

        app.MapControllers();

        try
        {
            Log.Information("Starting BookMarkBrain API");
            await app.RunAsync();
            return 0;
        }
        catch (Exception ex)
        {
            Log.Fatal(ex, "Host terminated unexpectedly");
            return 1;
        }
        finally
        {
            Log.CloseAndFlush();
        }
    }
}