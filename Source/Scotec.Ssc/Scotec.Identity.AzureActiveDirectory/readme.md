# Scotec.Identity.AzureActiveDirectory

Scotec.Identity.AzureActiveDirectory is a .NET library that provides authentication and token management for Azure Active Directory (AAD) using the Microsoft Authentication Library (MSAL). It supports secure token caching and seamless integration with Azure SDK clients, making it easy to acquire and manage AAD tokens in your applications.

## Features
- Acquire Azure AD tokens using MSAL
- Support for silent and interactive authentication flows
- Secure, persistent token caching
- Integration with Azure SDK clients via TokenCredential
- Configurable authentication options (client ID, tenant ID, scopes, token cache, auto sign-out)

## Usage
Configure authentication options with your Azure AD application details and required scopes. Use the provided services to acquire tokens or integrate with Azure SDKs.

## Example: Using AadAuthService and AadAuthSession with Azure Storageusing Azure.Storage.Blobs;
```csharp
using Scotec.Identity.AzureActiveDirectory;

// Configure authentication options
var options = new AadAuthOptions
{
    ClientId = "<your-client-id>",
    TenantId = "<your-tenant-id>",
    Scopes = new[] { "https://storage.azure.com/.default" },
    // Optionally, configure persistent token cache:
    // TokenCache = new TokenCache("<path-to-cache-file>")
};

// Create the authentication service
var authService = new AadAuthService(options);

// Get available accounts (optional)
var accounts = await authService.GetAccountsAsync();
var account = accounts.FirstOrDefault(); // Or select based on username

// Sign in (silent if possible, otherwise interactive)
var session = await authService.SignInAsync(account);

// Get a TokenCredential for Azure SDKs
var credential = session.TokenCredential;

// Use the credential with Azure BlobServiceClient
var blobServiceClient = new BlobServiceClient(new Uri("https://<your-storage-account>.blob.core.windows.net/"), credential);
```

Now you can use blobServiceClient to interact with Azure Blob Storage.
Replace `<your-client-id>`, `<your-tenant-id>`, and `<your-storage-account>` with your actual Azure AD and storage account details.

## License
MIT License. See `license.txt` for details.

For more information, visit the [Scotec Software Solutions website](https://www.scotec-software.com) or the [GitHub repository](https://github.com/scotec-Software-Solutions-AB/scotec_ssc).
