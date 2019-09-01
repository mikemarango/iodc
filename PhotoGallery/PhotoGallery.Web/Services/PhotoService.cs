﻿using IdentityModel.Client;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using Newtonsoft.Json;
using PhotoGallery.Data;
using PhotoGallery.Dto;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace PhotoGallery.Web.Services
{
    public class PhotoService
    {
        //private readonly HttpClient httpClient;
        //private readonly IHttpContextAccessor contextAccessor;
        //private readonly IConfiguration configuration;

        public PhotoService(
            HttpClient http, 
            IHttpContextAccessor accessor, 
            IConfiguration configuration)
        {
            //this.httpClient = httpClient;
            //this.contextAccessor = contextAccessor;
            //this.configuration = configuration;
            Client = new DiscoveryClient(configuration.GetConnectionString("identityServerUri"));
            Http = http;
            Accessor = accessor;
            Configuration = configuration;
        }

        public DiscoveryClient Client { get; }
        public HttpClient Http { get; }
        public IHttpContextAccessor Accessor { get; }
        public IConfiguration Configuration { get; }


        #region CRUD Tasks

        public async Task<IList<Photo>> GetPhotosAsync()
        {
            var httpClient = await GetClient();
            var response = await httpClient.GetAsync("api/photos");
            if (response.StatusCode == HttpStatusCode.Unauthorized || response.StatusCode == HttpStatusCode.Forbidden)
            {
                return null;
            }
            response.EnsureSuccessStatusCode();
            var photos = await response.Content.ReadAsAsync<IList<Photo>>();
            return photos;
        }

        public async Task<Photo> GetPhotoAsync(Guid id)
        {
            var httpClient = await GetClient();
            var response = await httpClient.GetAsync($"api/photos/{id}");
            if (response.StatusCode == HttpStatusCode.NotFound)
                return null;
            if (response.StatusCode == HttpStatusCode.Unauthorized || response.StatusCode == HttpStatusCode.Forbidden)
                return null;
            response.EnsureSuccessStatusCode();
            var photo = await response.Content.ReadAsAsync<Photo>();
            return photo;
        }

        public async Task UpdatePhotoAsync(Guid id, Photo photo)
        {
            var httpClient = await GetClient();
            var response = await httpClient.PutAsJsonAsync($"api/photos/{id}", photo);
            response.EnsureSuccessStatusCode();
        }

        public async Task<PhotoDto> AddPhotoAsync(PhotoCreateDto photoCreateDto)
        {
            var httpClient = await GetClient();
            var serializePhotoCreateDto = JsonConvert.SerializeObject(photoCreateDto);
            var response = await httpClient.PostAsync($"api/photos", new StringContent(serializePhotoCreateDto, Encoding.Unicode, "application/json"));
            response.EnsureSuccessStatusCode();
            var photoDto = await response.Content.ReadAsAsync<PhotoDto>();
            return photoDto;
        }

        public async Task DeletePhotoAsync(Guid id)
        {
            var httpClient = await GetClient();
            var response = await httpClient.DeleteAsync($"api/photos/{id}");
            if (response.StatusCode != HttpStatusCode.NoContent)
                return;
            response.EnsureSuccessStatusCode();
        }

        public async Task<string> GetAddressAsync()
        {
            //var discoverClient = new DiscoveryClient(configuration.GetConnectionString(""));
            var metadataResponse = await Client.GetAsync();
            var userInfoClient = new UserInfoClient(metadataResponse.UserInfoEndpoint);
            var accessToken = await Accessor.HttpContext.GetTokenAsync(OpenIdConnectParameterNames.AccessToken);
            var response = await userInfoClient.GetAsync(accessToken);
            if (response.IsError)
                throw new Exception("Problem accressing UserInfo endpoint");
            var address = response.Claims.FirstOrDefault(c => c.Type == "address")?.Value;
            return address;
        }

        #endregion

        #region HTTP Tasks

        public async Task<HttpClient> GetClient()
        {
            var accessToken = await GetValidAccessToken();
            if (!string.IsNullOrEmpty(accessToken))
                Http.SetBearerToken(accessToken);
            return await Task.FromResult(Http);
        }

        private async Task<string> GetValidAccessToken()
        {
            var currentContext = Accessor.HttpContext;
            var expiresAtToken = await currentContext.GetTokenAsync("expires_at");
            var expiresAt = string.IsNullOrWhiteSpace(expiresAtToken) ?
                DateTime.MinValue : DateTime.Parse(expiresAtToken).AddSeconds(-60).ToUniversalTime();
            var accessToken = await (expiresAt < DateTime.UtcNow ?
                RenewTokens() : currentContext.GetTokenAsync(OpenIdConnectParameterNames.AccessToken));
            return accessToken;
        }

        private async Task<string> RenewTokens()
        {
            var currentContext = Accessor.HttpContext;
            var discoveryClient = new DiscoveryClient(Configuration.GetConnectionString("identityServerUri"));
            //var discoveryClient = new DiscoveryClient(configuration.GetConnectionString("identityServerUri"));
            var metaDataResponse = await discoveryClient.GetAsync();
            var tokenClient = new TokenClient(metaDataResponse.TokenEndpoint, Configuration["ClientIdOnIdentityServer"], Configuration["ClientSecretOnIdentityServer"]);
            var currentRefreshToken = await currentContext.GetTokenAsync(OpenIdConnectParameterNames.RefreshToken);
            var tokenResult = await tokenClient.RequestRefreshTokenAsync(currentRefreshToken);
            if (!tokenResult.IsError)
            {
                // get current tokens
                var old_id_token = await currentContext.GetTokenAsync("id_token");
                var new_access_token = tokenResult.AccessToken;
                var new_refresh_token = tokenResult.RefreshToken;

                // get new tokens and expiration time
                var tokens = new List<AuthenticationToken>
                {
                    new AuthenticationToken { Name = OpenIdConnectParameterNames.IdToken, Value = old_id_token },
                    new AuthenticationToken { Name = OpenIdConnectParameterNames.AccessToken, Value = new_access_token },
                    new AuthenticationToken { Name = OpenIdConnectParameterNames.RefreshToken, Value = new_refresh_token }
                };

                var expiresAt = DateTime.UtcNow + TimeSpan.FromSeconds(tokenResult.ExpiresIn);
                tokens.Add(new AuthenticationToken { Name = "expires_at", Value = expiresAt.ToString("o", CultureInfo.InvariantCulture) });

                // store tokens and sign in with renewed tokens
                var info = await currentContext.AuthenticateAsync("Cookies");
                info.Properties.StoreTokens(tokens);
                await currentContext.SignInAsync("Cookies", info.Principal, info.Properties);

                // return the new access token 
                return tokenResult.AccessToken;
            }
            else
            {
                throw new Exception("Problem encountered while refreshing tokens.",
                    tokenResult.Exception);
            }
        }

        #endregion
    }
}
