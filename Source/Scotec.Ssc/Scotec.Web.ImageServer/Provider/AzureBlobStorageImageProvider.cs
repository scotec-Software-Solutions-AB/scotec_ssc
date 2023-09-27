using Azure.Storage.Blobs;

namespace Scotec.Web.ImageServer.Provider;

public class AzureBlobStorageImageProvider : IImageProvider
{
    private readonly string _connectionString; 

    public AzureBlobStorageImageProvider(IConfiguration configuration)
    {
        _connectionString = configuration.GetSection($"{nameof(AzureBlobStorageImageProvider)}:ConnectionString").Get<string>();
    }

    public async Task<Stream> GetImageAsync(string path)
    {
        try
        {
            var parts = path.Replace('\\', '/').Split('/', 2, StringSplitOptions.RemoveEmptyEntries);
            var blobServiceClient = new BlobServiceClient(_connectionString);

            var containerClient = blobServiceClient.GetBlobContainerClient(parts[0]);
            var blobClient = containerClient.GetBlobClient(parts[1]);
            var response = await blobClient.DownloadAsync().ConfigureAwait(true);
            
            return response.Value.Content;
        }
        catch (Exception e) when (e is not ImageServerException)
        {
            throw new ImageServerException($"Could not load image. Path:{path}", e);
        }
    }
}
