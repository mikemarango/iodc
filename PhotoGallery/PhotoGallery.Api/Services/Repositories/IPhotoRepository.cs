using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using PhotoGallery.Data;

namespace PhotoGallery.Api.Services.Repositories
{
    public interface IPhotoRepository
    {
        Task AddPhotoAsync(Photo image);
        Task DeletePhotoAsync(Photo image);
        Task<Photo> GetPhotoAsync(Guid id);
        Task<List<Photo>> GetPhotosAsync(string ownerId);
        Task UpdatePhotoAsync(Photo image);
        Task<bool> IsOwnersPhoto(Guid id, string ownerId);
    }
}