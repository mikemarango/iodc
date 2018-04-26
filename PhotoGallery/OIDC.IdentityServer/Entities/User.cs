using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OIDC.IdentityServer.Entities
{
    public class User
    {
        public string SubjectId { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public bool IsActive { get; set; }
        public ICollection<Claim> Claims { get; set; } = new List<Claim>();
        public ICollection<Login> Logins { get; set; } = new List<Login>();
    }
}
