using BookMarkBrain.Core;
using BookMarkBrain.Data;
using BookMarkBrain.MVC.Extensions;
using BookMarkBrain.MVC.Middleware;
using BookMarkBrain.MVC.Services;
using BookMarkBrain.MVC.Services.APIServiceImplementations;
using BookMarkBrain.MVC.Services.APIServiceInterfaces;
using BookMarkBrain.Services;
using Serilog;

namespace BookMarkBrain.MVC;

public class Program
{
    public static int Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        // Configure Serilog
        Log.Logger = new LoggerConfiguration()
            .ReadFrom.Configuration(builder.Configuration)
            .Enrich.FromLogContext()
            .WriteTo.Console()
            .WriteTo.File("logs/bookmarkbrain-mvc-.log", rollingInterval: RollingInterval.Day)
            .CreateLogger();

        builder.Host.UseSerilog();

        // Add services to the container
        builder.Services.AddCoreServices();
        builder.Services.AddDataServices(builder.Configuration);
        builder.Services.AddApplicationServices(builder.Configuration);
        builder.Services.AddMvcServices(builder.Configuration);

        // Add HttpClient services
        builder.Services.AddHttpClient("API", client =>
        {
            var apiBaseUrl = builder.Configuration["ApiSettings:BaseUrl"] ?? "https://localhost:5001/";
            client.BaseAddress = new Uri(apiBaseUrl);
            client.DefaultRequestHeaders.Add("Accept", "application/json");
            client.Timeout = TimeSpan.FromSeconds(30); // Set a reasonable timeout
        })
            .ConfigurePrimaryHttpMessageHandler(() => new HttpClientHandler
            {
                // For development, we might need to bypass SSL certificate validation
                ServerCertificateCustomValidationCallback = (sender, cert, chain, sslPolicyErrors) => true
            });


        // Configure MVC
        builder.Services
            .AddControllersWithViews()
            .AddRazorRuntimeCompilation()
            .AddNewtonsoftJson(options =>
            {
                options.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore;
            });

        // Add session services
        builder.Services.AddDistributedMemoryCache();
        builder.Services.AddSession(options =>
        {
            options.IdleTimeout = TimeSpan.FromMinutes(30);
            options.Cookie.HttpOnly = true;
            options.Cookie.IsEssential = true;
        });

        var app = builder.Build();

        // Configure the HTTP request pipeline
        if (app.Environment.IsDevelopment())
        {
            app.UseDeveloperExceptionPage();
        }
        else
        {
            app.UseExceptionHandler("/Home/Error");
            app.UseHsts();
        }

        app.UseHttpsRedirection();
        app.UseStaticFiles();
        app.UseRouting();
        app.UseSession();

        // Custom middleware
        app.UseMiddleware<RequestLoggingMiddleware>();

        app.MapControllerRoute(
            name: "default",
            pattern: "{controller=Home}/{action=Index}/{id?}");

        try
        {
            Log.Information("Starting BookMarkBrain MVC Web Application");
            app.Run();
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