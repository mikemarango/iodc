using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using PhotoGallery.Data;

namespace PhotoGallery.Api.Services.Repositories
{
    public interface IPhotoRepository
    {
        Task AddPhotoAsync(Photo image);
        Task DeletePhoto(Photo image);
        Task<Photo> GetPhotoAsync(Guid id);
        Task<List<Photo>> GetPhotosAsync();
        Task UpdatePhotoAsync(Photo image);
    }
}