using Mapster;
using MapsterMapper;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;


namespace BookMarkBrain.Services.Mappings;

public static class MappingConfig
{
    public static IServiceCollection AddMappingConfiguration(this IServiceCollection services)
    {
        // Register Mapster's mappings
        var config = TypeAdapterConfig.GlobalSettings;

        // Scan assemblies and register mappings
        config.Scan(Assembly.GetExecutingAssembly());

        // Register Mapster as service
        services.AddSingleton(config);
        services.AddScoped<IMapper, ServiceMapper>();

        return services;
    }
}
