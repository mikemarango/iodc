using Microsoft.AspNetCore.Authorization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PhotoGallery.Api.Authorization
{
    public class ImageOwnerRequirement : IAuthorizationRequirement
    {
        public ImageOwnerRequirement()
        {
        }
    }
}
