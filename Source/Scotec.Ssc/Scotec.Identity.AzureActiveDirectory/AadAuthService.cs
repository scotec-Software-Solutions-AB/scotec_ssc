using System.Collections.Concurrent;
using Azure.Core;
using Microsoft.Identity.Client;

namespace Scotec.Identity.AzureActiveDirectory;

/// <summary>
///     Provides authentication services using MSAL for acquiring Azure AD tokens in desktop applications.
/// </summary>
/// <remarks>
///     This service encapsulates the MSAL public client application and exposes methods for acquiring and managing tokens,
///     including silent and interactive flows, as well as sign-out functionality. It also exposes a
///     <see cref="TokenCredential" /> for use with Azure SDK clients.
/// </remarks>
public sealed class AadAuthService : IAadAuthService
{

    public IAadAuthSession GetSession(AadAuthOptions options)
    {
        return new AadAuthSession(options);
    }

}
