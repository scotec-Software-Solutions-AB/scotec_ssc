using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Scotec.Web.ImageServer.Test.Mocks;

namespace Scotec.Web.ImageServer.Test;

public class Startup
{
    public IServiceProvider ServiceProvider { get; private set; }
    
    public Startup()
    {
        var services = new ServiceCollection();
        var config = new ConfigurationBuilder()
                     .SetBasePath(AppContext.BaseDirectory)
                     .AddJsonFile("appsettings.json", false, true)
                     .Build();
        services.AddSingleton<IConfiguration>(config);

        services.AddImageServer()
                .AddLocalImageProvider("images")
                .AddAzureBlobStorageImageProvider("scotecblog");

        services.AddSingleton<IWebHostEnvironment, WebHostEnvironmentMock>();

        ServiceProvider = services.BuildServiceProvider();
    }
}
