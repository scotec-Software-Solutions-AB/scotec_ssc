using System.Security.Cryptography;
using Microsoft.Identity.Client;

namespace Scotec.Identity.AzureActiveDirectory;

/// <summary>
///     Provides a persistent, encrypted token cache for MSAL authentication tokens.
/// </summary>
/// <remarks>
///     This class enables secure storage and retrieval of authentication tokens using the Windows Data Protection API
///     (DPAPI).
///     The cache is stored in an encrypted file on disk, scoped to the current user.
///     It is thread-safe and can be used to enable token caching for MSAL clients.
/// </remarks>
public class TokenCache
{
    private static readonly object FileLock = new();
    private readonly string _cacheFile;

    /// <summary>
    ///     Initializes a new instance of the <see cref="TokenCache" /> class.
    /// </summary>
    /// <param name="cacheFile">The file path where the token cache will be stored.</param>
    /// <remarks>
    ///     The specified file will be used to persist the encrypted token cache.
    /// </remarks>
    public TokenCache(string cacheFile)
    {
        _cacheFile = cacheFile;
    }

    /// <summary>
    ///     Enables persistent caching for the specified MSAL token cache.
    /// </summary>
    /// <param name="tokenCache">The MSAL <see cref="ITokenCache" /> to enable persistence for.</param>
    /// <remarks>
    ///     This method hooks into the MSAL token cache events to provide secure, persistent storage.
    /// </remarks>
    public void Enable(ITokenCache tokenCache)
    {
        tokenCache.SetBeforeAccess(OnBeforeAccess);
        tokenCache.SetAfterAccess(OnAfterAccess);
    }

    /// <summary>
    ///     Handles the MSAL BeforeAccess event to load the token cache from disk.
    /// </summary>
    /// <param name="args">The token cache notification arguments.</param>
    /// <remarks>
    ///     If the cache file exists, it is decrypted and deserialized into the MSAL token cache.
    ///     If the file does not exist or an error occurs, the cache is treated as empty.
    /// </remarks>
    private void OnBeforeAccess(TokenCacheNotificationArgs args)
    {
        lock (FileLock)
        {
            if (!File.Exists(_cacheFile))
            {
                return;
            }

            try
            {
                var encrypted = File.ReadAllBytes(_cacheFile);
                var data = ProtectedData.Unprotect(encrypted, null, DataProtectionScope.CurrentUser);
                args.TokenCache.DeserializeMsalV3(data);
            }
            catch (CryptographicException)
            {
                // Decryption failed (e.g., cache was written by a different user/machine); treat as empty.
            }
            catch (IOException)
            {
                // File read error; treat cache as empty.
            }
        }
    }

    /// <summary>
    ///     Handles the MSAL AfterAccess event to persist the token cache to disk if it has changed.
    /// </summary>
    /// <param name="args">The token cache notification arguments.</param>
    /// <remarks>
    ///     If the token cache state has changed, it is serialized, encrypted, and written to the cache file.
    /// </remarks>
    private void OnAfterAccess(TokenCacheNotificationArgs args)
    {
        if (args.HasStateChanged)
        {
            lock (FileLock)
            {
                Directory.CreateDirectory(Path.GetDirectoryName(_cacheFile)!);
                var data = args.TokenCache.SerializeMsalV3();
                var encrypted = ProtectedData.Protect(data, null, DataProtectionScope.CurrentUser);
                File.WriteAllBytes(_cacheFile, encrypted);
            }
        }
    }
}
