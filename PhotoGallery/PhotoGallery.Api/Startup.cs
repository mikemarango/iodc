﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using IdentityServer4.AccessTokenValidation;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using PhotoGallery.Api.Services.Repositories;
using PhotoGallery.Data;
using PhotoGallery.Dto;

namespace PhotoGallery.Api
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
            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_1);
            services.AddDbContextPool<ApplicationContext>(options =>
                options.UseSqlServer(Configuration.GetConnectionString("defaultConnection"),
                b => b.MigrationsAssembly("PhotoGallery.Data")));
            services.AddScoped<IPhotoRepository, PhotoRepository>();
            services.AddAuthentication(IdentityServerAuthenticationDefaults.AuthenticationScheme)
                .AddIdentityServerAuthentication(options =>
                {
                    options.Authority = Configuration.GetConnectionString("identityServerUri");
                    options.RequireHttpsMetadata = true;
                    options.ApiName = "photogallery.api";
                    options.ApiSecret = "dcf84a90-98cf-48ec-af8b-50cb1f42d51b";
                });
            services.AddHttpContextAccessor();
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
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();
            Mapper.Initialize(config =>
            {
                config.CreateMap<Photo, PhotoDto>();
                config.CreateMap<PhotoCreateDto, Photo>();
                config.CreateMap<PhotoUpdateDto, Photo>();
            });
            app.UseAuthentication();

            app.UseMvc();
        }
    }
}
