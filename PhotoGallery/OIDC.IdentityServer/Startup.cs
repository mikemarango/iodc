// Copyright (c) Brock Allen & Dominick Baier. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.


using System;
using IdentityServer4;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using System.Security.Cryptography.X509Certificates;
using Microsoft.EntityFrameworkCore;
using OIDC.IdentityServer.Data;
using OIDC.IdentityServer.Services.Repository;
using OIDC.IdentityServer.Controllers;
using OIDC.IdentityServer.Services.Options;
using System.Reflection;

namespace OIDC.IdentityServer
{
    public class Startup
    {
        public IHostingEnvironment Environment { get; }
        public IConfiguration Configuration { get; }

        public Startup(IHostingEnvironment environment, IConfiguration configuration)
        {
            Environment = environment;
            Configuration = configuration;
        }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMvc();

            services.Configure<IISOptions>(options =>
            {
                options.AutomaticAuthentication = false;
                options.AuthenticationDisplayName = "Windows";
            });

            //services.Configure<FacebookAuth>(Configuration.GetSection("FacebookAuth"));

            var builder = services.AddIdentityServer(options =>
            {
                options.Events.RaiseErrorEvents = true;
                options.Events.RaiseInformationEvents = true;
                options.Events.RaiseFailureEvents = true;
                options.Events.RaiseSuccessEvents = true;
            })
            .AddUserStore()
            .AddSigningCredential(LoadCertificateFromStore(Configuration.GetConnectionString("oidc")))
            .AddConfigurationStore(options =>
            {
                options.ConfigureDbContext = context => context
                .UseSqlite(Configuration.GetConnectionString("ConfigurationConnection"), config =>
                {
                    config.MigrationsAssembly(typeof(Startup).GetTypeInfo().Assembly.GetName().Name);
                });
            })
            .AddOperationalStore(options =>
            {
                options.ConfigureDbContext = context => context
                .UseSqlite(Configuration.GetConnectionString("ConfigurationConnection"), config =>
                {
                    config.MigrationsAssembly(typeof(Startup).GetTypeInfo().Assembly.GetName().Name);
                });
            });
            
            //.AddTestUsers(TestUsers.Users);

            services.AddDbContextPool<IdsrvDbContext>(options =>
            {
                options.UseSqlite(Configuration.GetConnectionString("DefaultConnection"),
                    sqlOptions => sqlOptions.MigrationsAssembly("OIDC.IdentityServer"));
                //options.UseSqlite(Configuration.GetConnectionString("ConfigurationConnection"),
                //    sqlOptions => sqlOptions.MigrationsAssembly("OIDC.IdentityServer"));
            });

            services.AddScoped<IUserRepository, UserRepository>();

            services.AddHttpContextAccessor();

            //// in-memory, code config
            //builder.AddInMemoryIdentityResources(Config.GetIdentityResources());
            //builder.AddInMemoryApiResources(Config.GetApis());
            //builder.AddInMemoryClients(Config.GetClients());

            //// in-memory, json config
            //builder.AddInMemoryIdentityResources(Configuration.GetSection("IdentityResources"));
            //builder.AddInMemoryApiResources(Configuration.GetSection("ApiResources"));
            //builder.AddInMemoryClients(Configuration.GetSection("clients"));

            if (Environment.IsDevelopment())
            {
                builder.AddDeveloperSigningCredential();
            }

            else
            {
                throw new Exception("need to configure key material");
            }

            services.AddAuthentication()
                .AddGoogle(options =>
                {
                    options.SignInScheme = IdentityServerConstants.ExternalCookieAuthenticationScheme;

                    options.ClientId = Configuration["GoogleOath:ClientId"];
                    options.ClientSecret = Configuration["GoogleOath:ClientSecret"];
                })
                .AddCookie("idsrv.2FA")
                .AddFacebook(options =>
                {
                    options.AppId = Configuration["FacebookAuth:AppId"];
                    options.AppSecret = Configuration["FacebookAuth:AppSecret"];
                });
        }

        public X509Certificate2 LoadCertificateFromStore(string thumbPrint)
        {
            using (var store = new X509Store(StoreName.My, StoreLocation.LocalMachine))
            {
                store.Open(OpenFlags.ReadOnly);
                var certCollection = store.Certificates.Find(X509FindType.FindByThumbprint,
                    thumbPrint, true);
                if (certCollection.Count == 0)
                {
                    throw new Exception("The specified certificate wasn't found. Check the specified thumbprint.");
                }
                return certCollection[0];
            }
        }

        public void Configure(IApplicationBuilder app)
        {
            if (Environment.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            app.UseHttpsRedirection();
            app.UseIdentityServer();
            app.UseStaticFiles();
            app.UseMvcWithDefaultRoute();
        }
    }
}