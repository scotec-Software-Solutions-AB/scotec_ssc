

using Azure.Storage.Blobs;

namespace Scotec.Identity.AzureActiveDirectory.Test
{
    public class UnitTest1
    {
        [Fact]
        public void Test1()
        {
            var clientId = "d79c205d-6637-499c-a224-a90e6195eab9";
            var tenantId = "55ea1dea-56b6-4d91-be87-e452e768a583";

            var blobServiceEndpoint = "https://scotecfamilymanager.blob.core.windows.net/";
            var containerName = "families";

            var cacheFile = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
                "TestApp", "msal_cache.bin");

            var session = new AadAuthSession(new AuthOptions()
            {
                ClientId = clientId,
                TenantId = tenantId,
                Scopes = ["https://storage.azure.com/.default"],
                TokenCache = new TokenCache(cacheFile)
            });

            if (!session.IsSignedIn)
            {
                session.SignIn();
            }
            var tokenCred = session.TokenCredential;
            

            var containerClient = new BlobContainerClient(new Uri($"{blobServiceEndpoint}/{containerName}"), tokenCred);
            bool exists = containerClient.Exists();
           
            session.SignOut();
        }
    }
}