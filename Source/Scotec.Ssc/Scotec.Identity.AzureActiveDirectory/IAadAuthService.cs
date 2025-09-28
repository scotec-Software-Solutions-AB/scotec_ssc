using Microsoft.Identity.Client;

namespace Scotec.Identity.AzureActiveDirectory;

/// <summary>
///     Defines the contract for Azure Active Directory authentication services.
/// </summary>
public interface IAadAuthService
{
    IAadAuthSession GetSession(AadAuthOptions options);
}
