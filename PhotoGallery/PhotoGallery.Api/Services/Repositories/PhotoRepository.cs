using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authentication;
using PhotoGallery.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Security.Claims;

namespace PhotoGallery.Api.Services.Repositories
{
    public class PhotoRepository : IPhotoRepository
    {
        private readonly ApplicationContext context;

        public PhotoRepository(ApplicationContext context, IHostingEnvironment environment)
        {
            this.context = context;
        }

        public Task<List<Photo>> GetPhotosAsync()
        {
            return context.Photos.ToListAsync();
        }

        public Task<List<Photo>> GetPhotosAsync(string ownerId)
        {
            return context.Photos
                .Where(i => i.OwnerId == ownerId)
                .OrderBy(i => i.Title).ToListAsync();
        }

        public Task<Photo> GetPhotoAsync(Guid id)
        {
            return context.Photos.FirstOrDefaultAsync(i => i.Id == id);
        }

        public async Task AddPhotoAsync(Photo photo)
        {
            context.Photos.Add(photo);
            //context.Entry(photo).State = EntityState.Added;
            await Task.FromResult(context.SaveChangesAsync());
        }

        public Task UpdatePhotoAsync(Photo photo)
        {
            context.Entry(photo).State = EntityState.Modified;
            return context.SaveChangesAsync();
        }

        public Task DeletePhoto(Photo photo)
        {
            context.Photos.Remove(photo);
            return context.SaveChangesAsync();
        }

        public async Task<bool> IsOwnersPhoto(Guid id, string ownerId)
        {
            var photo = await context.Photos.AnyAsync(i => i.Id == id && i.OwnerId == ownerId);
            return await Task.FromResult(photo);
        }
    }
}
