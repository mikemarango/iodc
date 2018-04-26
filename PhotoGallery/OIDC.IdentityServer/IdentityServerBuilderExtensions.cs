using Microsoft.Extensions.DependencyInjection;
using OIDC.IdentityServer.Services.Profile;
using OIDC.IdentityServer.Services.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OIDC.IdentityServer
{
    public static class IdentityServerBuilderExtensions
    {
        public static IIdentityServerBuilder AddUserStore(this IIdentityServerBuilder serverBuilder)
        {
            serverBuilder.Services.AddScoped<IUserRepository, UserRepository>();
            serverBuilder.AddProfileService<OIDCUserProfileService>();
            return serverBuilder;
        }
    }
}
