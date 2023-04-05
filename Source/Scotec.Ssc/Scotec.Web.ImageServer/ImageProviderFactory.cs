namespace Scotec.Web.ImageServer
{
    public class ImageProviderFactory : IImageProviderFactory
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly IDictionary<string, Type> _descriptors;
        public ImageProviderFactory(IServiceProvider serviceProvider, IEnumerable<Tuple<string, Type>> descriptors)
        {
            _serviceProvider = serviceProvider;
            _descriptors = descriptors.ToDictionary(item => item.Item1, item => item.Item2);
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
