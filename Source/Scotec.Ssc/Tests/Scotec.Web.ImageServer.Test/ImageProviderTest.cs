using Scotec.Web.ImageServer.Server;
using Xunit;
using Microsoft.Extensions.DependencyInjection;

namespace Scotec.Web.ImageServer.Test;

public class ImageProviderTest : IClassFixture<Startup>
{
    private readonly ITestOutputHelper _output;

    public ImageProviderTest(Startup startup)
    {
        
        ImageServer = startup.ServiceProvider.GetService<IImageServer>();
    }

    private IImageServer ImageServer { get; }

    [Theory]
    [InlineData("images/Logo.png")]
    public async Task GetImageAsync(string path)
    {
        try
        {
            var image = await ImageServer.GetImageAsync(path);
            var imageFromCache = await ImageServer.GetImageAsync(path);
        }
        catch (Exception e)
        {
            _output.WriteLine("GetImageAsync failed", e);
            throw;
        }
    }
}
