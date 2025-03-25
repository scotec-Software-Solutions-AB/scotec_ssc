using System.Reflection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Configuration.Json;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Scotec.Extensions.Utilities.Strings;
using Microsoft.Extensions.DependencyInjection;

#if NET8_0_OR_GREATER

namespace Scotec.Extensions.Utilities.Configuration;

public static class HostBuilderExtensions
{
    public static IHostBuilder AddUserSettings(this IHostBuilder hostBuilder, IConfiguration configuration)
    {
        //var configurationBuilder = new ConfigurationBuilder()
        //              .SetBasePath(AppContext.BaseDirectory) // Set the base path to the application's directory
        //              .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true) // Load appsettings.json
        //              .AddJsonFile($"appsettings.{Environment.GetEnvironmentVariable($"{environment.ToString()}")}.json", optional: true, reloadOnChange: true) // Load environment-specific appsettings
        //              .AddEnvironmentVariables(); // Add environment variables
        //// Build the configuration
        //IConfiguration configuration = configurationBuilder.Build();
        // Example: Accessing a configuration value

       // var basePath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location)!;

        var options = configuration.GetSection("SettingsManager").Get<SettingsManagerOptions>();
        if (string.IsNullOrWhiteSpace(options?.SettingsFile))
        {
            throw new InvalidOperationException("Settings file path is not specified in the application settings.");
        }

        var optionsTypes = SettingsManager.GetOptionTypes();

        hostBuilder.ConfigureHostConfiguration(configBuilder => { configBuilder.AddJsonFile(options.SettingsFile.ExpandVariables(), false, true); });

        hostBuilder.ConfigureServices((hostContext, services) =>
        {
            foreach (var (sectionName, type) in optionsTypes)
            {
                services.Configure(type, hostContext.Configuration.GetSection(sectionName));
            }
        });

        return hostBuilder;
    }

    private static void Configure(this IServiceCollection services, Type optionsType, IConfigurationSection section)
    {
        // Get the generic Configure<T> method
        var configureMethod = typeof(OptionsConfigurationServiceCollectionExtensions)
            .GetMethod("Configure", [typeof(IServiceCollection), typeof(IConfiguration)]);

        if (configureMethod == null)
        {
            throw new InvalidOperationException("Unable to find Configure<T> method.");
        }

        // Make the method generic with the runtime type
        var genericMethod = configureMethod.MakeGenericMethod(optionsType);
        // Invoke the method with the provided services and configuration section
        genericMethod.Invoke(null, [services, section]);
    }

}

#endif