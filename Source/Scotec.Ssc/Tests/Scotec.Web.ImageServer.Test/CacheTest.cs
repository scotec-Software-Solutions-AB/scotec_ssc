using Scotec.Web.ImageServer.Server;
using Xunit;
using Xunit.Abstractions;
using Xunit.DependencyInjection;

namespace Scotec.Web.ImageServer.Test;

public class CacheTest
{
    private readonly ITestOutputHelper _output;

    public CacheTest(IImageServer imageServer, ITestOutputHelperAccessor outputAccessor)
    {
        _output = outputAccessor.Output;
        ImageServer = imageServer;
    }

    private IImageServer ImageServer { get; }

    [Fact]
    public async void GetImagesAsync()
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
