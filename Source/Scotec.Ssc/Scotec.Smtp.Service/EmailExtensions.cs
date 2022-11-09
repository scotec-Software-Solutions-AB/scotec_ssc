using Microsoft.Extensions.DependencyInjection;

namespace Scotec.Smtp.Service;

public static class EmailExtensions
{
    public static IServiceCollection AddEmailServices(this IServiceCollection services)
    {
        services.AddSingleton<IEmailDispatcher, EmailDispatcher>();
        services.AddScoped<IEmailService, EmailService>();
        services.AddScoped<IEmailBuffer, EmailBuffer>();
        services.AddTransient<IEmailClient, EmailClient>();

        return services;
    }
}