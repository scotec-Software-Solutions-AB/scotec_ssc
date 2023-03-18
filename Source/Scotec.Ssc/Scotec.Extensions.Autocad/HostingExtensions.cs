using Microsoft.Extensions.DependencyInjection;

namespace Scotec.Extensions.Autocad;

public static class HostingExtensions
{
    public static IServiceCollection AddLayerManager(this IServiceCollection services)
    {
        return services.AddSingleton<ILayerManager, LayerManager>();
    }
}
