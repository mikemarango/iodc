using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OIDC.IdentityServer.Entities
{
    public class Claim
    {
        public Claim() { }
        public Claim(string claimType, string claimValue)
        {
            ClaimType = claimType;
            ClaimValue = claimValue;
        }
        public string Id { get; set; }
        public string SubjectId { get; set; }
        public string ClaimType { get; set; }
        public string ClaimValue { get; set; }
    }
}
