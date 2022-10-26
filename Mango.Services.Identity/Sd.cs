using Duende.IdentityServer;
using Duende.IdentityServer.Models;

namespace Mango.Services.Identity;

/// <summary>
/// Static Details - Constants
/// </summary>
public static class Sd
{
    public const string Admin = "Admin";
    public const string Customer = "Customer";

    public static IEnumerable<IdentityResource> IdentityResources => new List<IdentityResource>
    {
        new IdentityResources.OpenId(),
        new IdentityResources.Email(),
        new IdentityResources.Profile()
    };

    public static IEnumerable<ApiScope> ApiScopes => new List<ApiScope>
    {
        new ApiScope("mango", "Mango Server"),
        new ApiScope("read", "Read data"),
        new ApiScope("write", "Write data"),
        new ApiScope("delete", "Delete data"),
    };

    public static IEnumerable<Client> Clients => new List<Client>
    {
        new Client()
        {
            ClientId = "client",
            ClientSecrets = { new Secret("secret".Sha256()) },
            AllowedGrantTypes = GrantTypes.ClientCredentials,
            AllowedScopes = { "read", "write", IdentityServerConstants.StandardScopes.Profile }
        },
        new Client()
        {
            ClientId = "mango",
            ClientSecrets = { new Secret("secret".Sha256()) },
            AllowedGrantTypes = GrantTypes.Code,
            RedirectUris = { "https://localhost:7247/signin-oidc" },
            PostLogoutRedirectUris = { "https://localhost:7247/signout-callback-oidc" },
            AllowedScopes =
            {
                IdentityServerConstants.StandardScopes.OpenId,
                IdentityServerConstants.StandardScopes.Email,
                IdentityServerConstants.StandardScopes.Profile,
                "mango"
            }
        },
    };
}