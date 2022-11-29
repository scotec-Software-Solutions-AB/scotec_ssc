using Microsoft.Extensions.DependencyInjection;
using Scotec.Smtp.Service.NoBuffer;

namespace Scotec.Smtp.Service;

public static class EmailExtensions
{
    /// <summary>
    /// Added the no-buffer strategy.
    /// </summary>
    public static IServiceCollection AddNoBufferEmailServices(this IServiceCollection services)
    {
        services.AddEmailServices();

        services.AddSingleton<INoBufferEmailDispatcher, NoBufferEmailDispatcher>();
        services.AddScoped<IEmailBuffer, NoBufferEmailBuffer>();

        return services;
    }

    /// <summary>
    /// Adds basic email services.
    /// </summary>
    public static IServiceCollection AddEmailServices(this IServiceCollection services)
    {
        services.AddScoped<IEmailService, EmailService>();
        services.AddTransient<IEmailClient, EmailClient>();

        return services;
    }
}