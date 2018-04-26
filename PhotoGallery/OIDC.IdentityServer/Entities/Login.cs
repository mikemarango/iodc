using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OIDC.IdentityServer.Entities
{
    public class Login
    {
        public string Id { get; set; }
        public string SubjectId { get; set; }
        public string LoginProvider { get; set; }
        public string ProviderKey { get; set; }
    }
}
