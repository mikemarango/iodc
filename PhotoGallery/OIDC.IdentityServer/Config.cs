// Copyright (c) Brock Allen & Dominick Baier. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.


using IdentityServer4.Models;
using System.Collections.Generic;

namespace OIDC.IdentityServer
{
    public static class Config
    {
        public static IEnumerable<IdentityResource> GetIdentityResources()
        {
            return new IdentityResource[]
            {
                new IdentityResources.OpenId(),
                new IdentityResources.Profile(),
                new IdentityResources.Address(),
                new IdentityResource("roles", "Your role(s)", new[] { "role" }),
                new IdentityResource("subscriptionlevel", "Your subscription level", new[] { "subscriptionlevel" }),
                new IdentityResource("country", "Your country", new[] { "country" })
            };
        }

        public static IEnumerable<ApiResource> GetApis()
        {
            return new ApiResource[]
            {
                new ApiResource("photogallery.api", "Photo Gallery API", new[] { "role" } )
                {
                    ApiSecrets = new[] { new Secret("dcf84a90-98cf-48ec-af8b-50cb1f42d51b".Sha256()) }
                }
            };
        }

        public static IEnumerable<Client> GetClients()
        {
            return new[]
            {
                // client credentials flow client
                new Client
                {
                    ClientId = "client",
                    ClientName = "Client Credentials Client",

                    AllowedGrantTypes = GrantTypes.ClientCredentials,
                    ClientSecrets = { new Secret("511536EF-F270-4058-80CA-1C89C192F69A".Sha256()) },

                    AllowedScopes = { "api1" }
                },

                // MVC client using hybrid flow
                new Client
                {
                    ClientId = "photogallery.web",
                    ClientName = "Photo Gallery",

                    AllowedGrantTypes = GrantTypes.Hybrid,
                    ClientSecrets = { new Secret("49C1A7E1-0C79-4A89-A3D6-A37998FB86B0".Sha256()) },

                    AccessTokenType = AccessTokenType.Reference,
                    AccessTokenLifetime = 120,

                    RedirectUris = { "https://localhost:44309/signin-oidc" },
                    FrontChannelLogoutUri = "https://localhost:44309/signout-oidc",
                    PostLogoutRedirectUris = { "https://localhost:44309/signout-callback-oidc" },

                    AllowOfflineAccess = true,
                    AllowedScopes = { "openid", "profile", "address", "roles", "photogallery.api", "subscriptionlevel", "country" },
                    AlwaysIncludeUserClaimsInIdToken = true,

                    UpdateAccessTokenClaimsOnRefresh = true,

                },

                // SPA client using implicit flow
                new Client
                {
                    ClientId = "spa",
                    ClientName = "SPA Client",
                    ClientUri = "http://identityserver.io",

                    AllowedGrantTypes = GrantTypes.Implicit,
                    AllowAccessTokensViaBrowser = true,

                    RedirectUris =
                    {
                        "http://localhost:5002/index.html",
                        "http://localhost:5002/callback.html",
                        "http://localhost:5002/silent.html",
                        "http://localhost:5002/popup.html",
                    },

                    PostLogoutRedirectUris = { "http://localhost:5002/index.html" },
                    AllowedCorsOrigins = { "http://localhost:5002" },

                    AllowedScopes = { "openid", "profile", "api1" }
                }
            };
        }
    }
}