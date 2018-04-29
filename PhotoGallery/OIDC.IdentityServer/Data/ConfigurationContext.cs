using IdentityServer4.EntityFramework.DbContexts;
using IdentityServer4.EntityFramework.Entities;
using IdentityServer4.EntityFramework.Mappers;
using IdentityServer4.EntityFramework.Options;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OIDC.IdentityServer.Data
{
    public class ConfigurationContext : ConfigurationDbContext
    {
        public ConfigurationContext(DbContextOptions<ConfigurationDbContext> options, ConfigurationStoreOptions storeOptions) : base(options, storeOptions)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Client>().HasData(GetClientData());
            modelBuilder.Entity<IdentityResource>().HasData(GetIdentityResourcesData());
            modelBuilder.Entity<ApiResource>().HasData(GetApiResourcesData());

            base.OnModelCreating(modelBuilder);
        }

        private ApiResource[] GetApiResourcesData()
        {
            foreach (var resource in Config.GetApis())
            {
                ApiResources.Add(resource.ToEntity());
            }
            return ApiResources.ToArray();
        }

        private IdentityResource[] GetIdentityResourcesData()
        {
            foreach (var resource in Config.GetIdentityResources())
            {
                IdentityResources.Add(resource.ToEntity());
            }
            return IdentityResources.ToArray();
        }

        private async Task<Client[]> GetClientData()
        {
            foreach (var client in Config.GetClients())
            {
                Clients.Add(client.ToEntity());
            }

            return await Clients.ToArrayAsync();
        }

        
    }
}
