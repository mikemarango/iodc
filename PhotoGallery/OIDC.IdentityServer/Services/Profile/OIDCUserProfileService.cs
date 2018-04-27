using IdentityServer4.Extensions;
using IdentityServer4.Models;
using IdentityServer4.Services;
using OIDC.IdentityServer.Services.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace OIDC.IdentityServer.Services.Profile
{
    public class OIDCUserProfileService : IProfileService
    {
        public OIDCUserProfileService(IUserRepository userRepository)
        {
            UserRepository = userRepository;
        }

        public IUserRepository UserRepository { get; }

        public async Task GetProfileDataAsync(ProfileDataRequestContext context)
        {
            var subjectId = context.Subject.GetSubjectId();
            var userClaims = await UserRepository.GetUserClaimsBySubjectIdAsync(subjectId);
            context.IssuedClaims = userClaims.Select(claim => new Claim(claim.ClaimType, claim.ClaimValue)).ToList();
        }

        public async Task IsActiveAsync(IsActiveContext context)
        {
            var subjectId = context.Subject.GetSubjectId();
            context.IsActive = await UserRepository.IsUserActiveAsync(subjectId);
        }
    }
}
