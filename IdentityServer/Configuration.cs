using IdentityModel;
using IdentityServer4.Models;

namespace IdentityServer
{
    public static class Configuration
    {
        // Identity Resources (Scopes).
        public static IEnumerable<IdentityResource> GetIdentityResources() =>
            new List<IdentityResource>
            {
                new IdentityResources.OpenId(),
                // new IdentityResources.Profile(),
                new IdentityResource
                {
                    Name = "rc.scope",
                    UserClaims =
                    {
                        "rc.grandma"
                    }
                }
            };

        // Set Api that let Clients access.
        public static IEnumerable<ApiResource> GetApis() =>
            new List<ApiResource> { 
                new ApiResource
                {
                    Name = "ApiOne",
                    Scopes = { "ApiOne" },
                    UserClaims = { "rc.api.grandma" }
                },
                new ApiResource 
                {
                    Name = "ApiTwo",
                    Scopes = { "ApiTwo" },
                    UserClaims = { "rc.api.grandma" }
                }
            };

        // Set Clients to get Access Token.
        public static IEnumerable<Client> GetClients() =>
            new List<Client> { 
                new Client { 
                    ClientId = "client_id",
                    ClientSecrets = { new Secret("client_secret".ToSha256()) },
                    AllowedGrantTypes = GrantTypes.ClientCredentials,
                    AllowedScopes = { "ApiOne" }
                }, // ClientCredentials
                new Client {
                    ClientId = "client_id_mvc",
                    ClientSecrets = { new Secret("client_secret_mvc".ToSha256()) },
                    AllowedGrantTypes = GrantTypes.Code,
                    RedirectUris = { "https://localhost:44322/signin-oidc"},    // Callback to McvClient that access to IdentityServer (Accout Login Page).
                    AllowedScopes = { 
                        "ApiOne", 
                        "ApiTwo",  
                        IdentityServer4.IdentityServerConstants.StandardScopes.OpenId,
                        // IdentityServer4.IdentityServerConstants.StandardScopes.Profile,
                        "rc.scope",
                    },

                    // Load claims to ID Token & Access Token.
                    // AlwaysIncludeUserClaimsInIdToken = true,

                    AllowOfflineAccess = true,

                    RequireConsent = false,
                }, // Authorization Code
                new Client {
                    ClientId = "client_id_js",
                    AllowedGrantTypes = GrantTypes.Implicit,
                    RedirectUris = { "https://localhost:44345/home/signin" },
                    AllowedScopes = {
                        IdentityServer4.IdentityServerConstants.StandardScopes.OpenId,
                        "ApiOne",
                    },

                    AllowAccessTokensViaBrowser = true,
                    RequireConsent = false,
                } // Implicit 
            };

        // Set Scope variable.
        public static List<ApiScope> Scopes = new List<ApiScope>
        {
            new ApiScope { Name = "ApiOne" },
            new ApiScope { Name = "ApiTwo" }
        };

        // Set Resources variable.
        public static List<ApiResource> Resources = new List<ApiResource>
        {
            new ApiResource { Name = "ApiOne" },
            new ApiResource { Name = "ApiTwo" }
        };
    }
}
