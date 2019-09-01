﻿using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using IdentityModel.Client;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using PhotoGallery.Data;
using PhotoGallery.Web.Services;

namespace PhotoGallery.Web
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.Configure<CookiePolicyOptions>(options =>
            {
                // This lambda determines whether user consent for non-essential cookies is needed for a given request.
                options.CheckConsentNeeded = context => true;
                options.MinimumSameSitePolicy = SameSiteMode.None;
            });

            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = "Cookies";
                options.DefaultChallengeScheme = "oidc";
            })
            .AddCookie("Cookies", options =>
            {
                options.AccessDeniedPath = "/Account/AccessDenied";
            })
            .AddOpenIdConnect("oidc", options =>
            {
                options.SignInScheme = "Cookies";
                options.Authority = Configuration.GetConnectionString("identityServerUri");
                options.RequireHttpsMetadata = true;
                options.ClientId = "photogallery.web";
                options.ClientSecret = "49C1A7E1-0C79-4A89-A3D6-A37998FB86B0";
                options.Scope.Add("openid");
                options.Scope.Add("profile");
                options.Scope.Add("address");
                options.Scope.Add("roles");
                options.Scope.Add("photogallery.api");
                options.ResponseType = "code id_token";
                options.SaveTokens = true;
                options.GetClaimsFromUserInfoEndpoint = true;
                options.Events = new OpenIdConnectEvents()
                {
                    OnTicketReceived = ticketReceivedContext =>
                    {
                        return Task.CompletedTask;
                    },

                    OnTokenValidated = tokenValidatedContext =>
                    {
                        var identity = tokenValidatedContext.Principal.Identity
                            as ClaimsIdentity;

                        var targetClaims = identity.Claims.Where(z => new[] { "subscriptionlevel", "country", "role", "sub" }.Contains(z.Type));

                        var newClaimsIdentity = new ClaimsIdentity(
                          targetClaims,
                          identity.AuthenticationType,
                          "given_name",
                          "role");

                        tokenValidatedContext.Principal =
                            new ClaimsPrincipal(newClaimsIdentity);

                        return Task.CompletedTask;
                    },

                    OnUserInformationReceived = userInformationReceivedContext =>
                    {
                        userInformationReceivedContext.User.Remove("address");
                        return Task.FromResult(0);
                    }

                };
            });

            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_1);
            //services.AddDbContextPool<ApplicationContext>(options =>
            //    options.UseSqlServer(Configuration.GetConnectionString("defaultConnection")));
            services.AddHttpContextAccessor();
            services.AddHttpClient<PhotoService>(client =>
            {
                client.BaseAddress = new Uri(Configuration.GetConnectionString("photosApiUri"));
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Add("Accept", "application/json");
            });
            //services.AddHttpClient<DiscoveryClient>(disco =>
            //{
            //    disco.BaseAddress = new Uri(Configuration.GetConnectionString("identityServerUri"));
            //});
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                app.UseHsts();
            }

            JwtSecurityTokenHandler.DefaultInboundClaimTypeMap.Clear();
            JwtSecurityTokenHandler.DefaultInboundClaimFilter.Clear();


            app.UseHttpsRedirection();
            app.UseStaticFiles();
            app.UseCookiePolicy();
            app.UseAuthentication();
            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Photos}/{action=Index}/{id?}");
            });
        }
    }
}
