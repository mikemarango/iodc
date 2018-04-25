using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.Filters;
using PhotoGallery.Api.Services.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PhotoGallery.Api.Authorization
{
    public class ImageOwnerHandler : AuthorizationHandler<ImageOwnerRequirement>
    {
        public ImageOwnerHandler(IPhotoRepository photoRepository)
        {
            PhotoRepository = photoRepository;
        }

        public IPhotoRepository PhotoRepository { get; }

        protected override async Task HandleRequirementAsync(AuthorizationHandlerContext context, ImageOwnerRequirement requirement)
        {
            var filterContext = context.Resource as AuthorizationFilterContext;
            if (filterContext == null)
            {
                context.Fail();
                await Task.CompletedTask;
            }

            var imageId = filterContext.RouteData.Values["id"].ToString();

            Guid guid;

            if (!Guid.TryParse(input: imageId, result: out guid))
            {
                context.Fail();
                await Task.FromResult(0);
            }


            var ownerId = context.User.Claims.FirstOrDefault(c => c.Type == "sub").Value;

            if (!await PhotoRepository.IsOwnersPhoto(guid, ownerId))
            {
                context.Fail();
                await Task.FromResult(0);
            }

            context.Succeed(requirement);

            await Task.FromResult(0);
        }
    }
}
