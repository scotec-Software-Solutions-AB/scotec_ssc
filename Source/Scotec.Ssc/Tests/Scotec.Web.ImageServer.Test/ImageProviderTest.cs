using Scotec.Web.ImageServer.Server;
using Xunit;
using Xunit.Abstractions;
using Xunit.DependencyInjection;

namespace Scotec.Web.ImageServer.Test;

public class ImageProviderTest
{
    private readonly ITestOutputHelper _output;

    public ImageProviderTest(IImageServer imageServer, ITestOutputHelperAccessor outputAccessor)
    {
        _output = outputAccessor.Output;
        ImageServer = imageServer;
    }

    private IImageServer ImageServer { get; }

    [Theory]
    //[InlineData("images/Logo.png")]
    [InlineData("scotecblog/Logo.png")]
    public async void GetImageAsync(string path)
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
