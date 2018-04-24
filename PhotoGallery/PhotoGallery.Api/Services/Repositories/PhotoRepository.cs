using Microsoft.EntityFrameworkCore;
using PhotoGallery.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PhotoGallery.Api.Services.Repositories
{
    public class PhotoRepository : IPhotoRepository
    {
        private readonly ApplicationContext context;

        public PhotoRepository(ApplicationContext context)
        {
            this.context = context;
        }

        public Task<List<Photo>> GetPhotosAsync()
        {
            return context.Photos.ToListAsync();
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
    }
}
