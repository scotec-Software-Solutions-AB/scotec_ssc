using Scotec.Web.ImageServer.Server;
using Xunit;
using Microsoft.Extensions.DependencyInjection;

namespace Scotec.Web.ImageServer.Test;

public class CacheTest : IClassFixture<Startup>
{
    private readonly ITestOutputHelper _output;

    public CacheTest(Startup startup, ITestOutputHelper output)
    {
        _output = output;
        ImageServer = startup.ServiceProvider.GetRequiredService<IImageServer>();
    }

    private IImageServer ImageServer { get; }

    [Fact]
    public async Task GetImagesAsync()
    {
        try
        {
            for (int i = 1; i <= 3; i++)
            {
                var path = $"images/Logo{i}.png";
                var image1 = await ImageServer.GetImageAsync(path);
            }
        }
        catch (Exception e)
        {
            _output.WriteLine("GetImageAsync failed", e);
            throw;
        }
    }
}
