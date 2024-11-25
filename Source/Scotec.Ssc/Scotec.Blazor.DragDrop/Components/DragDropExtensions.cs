using Microsoft.Extensions.DependencyInjection;

namespace Scotec.Blazor.DragDrop.Components;

public static class DragDropExtensions
{
    public static IServiceCollection AddBlazorDragDrop(this IServiceCollection services)
    {
        return services.AddScoped(typeof(Clipboard<>));
    }
}
