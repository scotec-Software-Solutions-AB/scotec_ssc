namespace Scotec.Web.ImageServer
{
    public class ImageProviderFactory : IImageProviderFactory
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly IDictionary<string, Type> _descriptors;
        public ImageProviderFactory(IServiceProvider serviceProvider, IEnumerable<ImageProviderDescriptor> descriptors)
        {
            _serviceProvider = serviceProvider;
            _descriptors = descriptors.ToDictionary(descriptor => descriptor.Path, item => item.ImageProviderType);
        }
        public IImageProvider? CreateImageProvider(ImageRequest request)
        {
            var key = request.Path.Split('/', 2, StringSplitOptions.RemoveEmptyEntries).FirstOrDefault();
            if (key != null && _descriptors.TryGetValue(key, out var type))
            {
                return (IImageProvider?)_serviceProvider.GetService(type);
            }

            return null;
        }
    }
}
