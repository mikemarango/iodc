﻿using System.Collections.Generic;
using System.Threading.Tasks;
using OIDC.IdentityServer.Entities;

namespace OIDC.IdentityServer.Services.Repository
{
    public interface IUserRepository
    {
        Task AddUser(User user);
        Task AddUserClaim(string subjectId, string claimType, string claimValue);
        Task AddUserLogin(string subjectId, string loginProvider, string providerKey);
        Task<User> GetUserByEmailAsync(string email);
        Task<User> GetUserByProviderAsync(string loginProvider, string providerKey);
        Task<User> GetUserBySubjectIdAsync(string subjectId);
        Task<User> GetUserByUsernameAsync(string username);
        Task<IEnumerable<Claim>> GetUserClaimsBySubjectId(string subjectId);
        Task<IEnumerable<Login>> GetUserLoginsBySubjectId(string subjectId);
        Task<bool> IsUserActiveAsync(string subjectId);
        Task<bool> SaveAsync();
        Task<bool> ValidateUserCredentials(string username, string password);
    }
}