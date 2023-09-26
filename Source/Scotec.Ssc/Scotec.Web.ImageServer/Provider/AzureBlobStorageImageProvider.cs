using Azure.Storage.Blobs;

namespace Scotec.Web.ImageServer.Provider;

public class AzureBlobStorageImageProvider : IImageProvider
{
    private readonly string _connectionString; 

    private readonly string _blobContainerName;

    public AzureBlobStorageImageProvider(IConfiguration configuration)
    {
        _connectionString = configuration.GetSection($"{nameof(AzureBlobStorageImageProvider)}.ConnectionString").Get<string>();
        _blobContainerName = configuration.GetSection($"{nameof(AzureBlobStorageImageProvider)}.ContainerName").Get<string>();
    }

    public async Task<Stream?> GetImageAsync(string path)
    {
        try
        {
            var blobServiceClient = new BlobServiceClient(_connectionString);

            var containerClient = blobServiceClient.GetBlobContainerClient(_blobContainerName);
            var blobClient = containerClient.GetBlobClient(path);
            var response = await blobClient.DownloadAsync();
            
            return response.Value.Content;
        }
        catch (Exception)
        {
            return default;
        }
    }
}
