using OIDC.IdentityServer.Data;
using OIDC.IdentityServer.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace OIDC.IdentityServer.Services.Repository
{
    public class UserRepository : IUserRepository
    {
        public UserRepository(IdsrvDbContext context)
        {
            Context = context;
        }

        public IdsrvDbContext Context { get; }

        public async Task<User> GetUserBySubjectIdAsync(string subjectId)
        {
            return await Context.Users
                .FirstOrDefaultAsync(u => u.SubjectId == subjectId);
        }
        public async Task<User> GetUserByEmailAsync(string email)
        {
            return await Context.Users
                .FirstOrDefaultAsync(u => u.Claims
                .Any(c => c.ClaimType == "email" && c.ClaimValue == email));
        }
        public async Task<User> GetUserByProviderAsync(string loginProvider, string providerKey)
        {
            return await Context.Users
                .FirstOrDefaultAsync(u => u.Logins
                .Any(l => l.LoginProvider == loginProvider && l.ProviderKey == providerKey));
        }
        public async Task<User> GetUserByUsernameAsync(string username)
        {
            return await Context.Users.FirstOrDefaultAsync(u => u.Username == username);
        }
        public async Task<IEnumerable<Claim>> GetUserClaimsBySubjectIdAsync(string subjectId)
        {
            var user = await Context.Users.Include("Claims").FirstOrDefaultAsync(u => u.SubjectId == subjectId);
            if (user == null)
            {
                return new List<Claim>();
            }
            return user.Claims.ToList();
        }
        public async Task<IEnumerable<Login>> GetUserLoginsBySubjectIdAsync(string subjectId)
        {
            var user = await Context.Users.Include("Logins")
                .FirstOrDefaultAsync(u => u.SubjectId == subjectId);
            return user.Logins.ToList();
        }

        public async Task AddUserAsync(User user)
        {
            await Context.Users.AddAsync(user);
            await Context.SaveChangesAsync();
        }
        public async Task AddUserLoginAsync(string subjectId, string loginProvider, string providerKey)
        {
            var user = await GetUserBySubjectIdAsync(subjectId);
            if (user == null)
                throw new ArgumentNullException("User with given subject not found.", subjectId);
            user.Logins.Add(new Login()
            {
                SubjectId = subjectId,
                LoginProvider = loginProvider,
                ProviderKey = providerKey
            });
            await Context.SaveChangesAsync();
        }
        public async Task AddUserClaimAsync(string subjectId, string claimType, string claimValue)
        {
            var user = await GetUserBySubjectIdAsync(subjectId);
            if (user == null)
                throw new ArgumentNullException(subjectId);
            user.Claims.Add(new Claim(claimType, claimValue));
            await Context.SaveChangesAsync();
        }

        public async Task<bool> ValidateUserCredentialsAsync(string username, string password)
        {
            var user = await GetUserByUsernameAsync(username);
            if (user == null) return false;
            return (user.Password == password && !string.IsNullOrWhiteSpace(password));
        }
        public async Task<bool> IsUserActiveAsync(string subjectId)
        {
            var user = await GetUserBySubjectIdAsync(subjectId);
            return user.IsActive;
        }
        public async Task<bool> SaveAsync()
        {
            return await Context.SaveChangesAsync() >= 0;
        }
    }
}
