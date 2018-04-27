using System.Collections.Generic;
using System.Threading.Tasks;
using OIDC.IdentityServer.Entities;

namespace OIDC.IdentityServer.Services.Repository
{
    public interface IUserRepository
    {
        Task AddUserAsync(User user);
        Task AddUserClaimAsync(string subjectId, string claimType, string claimValue);
        Task AddUserLoginAsync(string subjectId, string loginProvider, string providerKey);
        Task<User> GetUserByEmailAsync(string email);
        Task<User> GetUserByProviderAsync(string loginProvider, string providerKey);
        Task<User> GetUserBySubjectIdAsync(string subjectId);
        Task<User> GetUserByUsernameAsync(string username);
        Task<IEnumerable<Claim>> GetUserClaimsBySubjectIdAsync(string subjectId);
        Task<IEnumerable<Login>> GetUserLoginsBySubjectIdAsync(string subjectId);
        Task<bool> IsUserActiveAsync(string subjectId);
        Task<bool> SaveAsync();
        Task<bool> ValidateUserCredentialsAsync(string username, string password);
    }
}