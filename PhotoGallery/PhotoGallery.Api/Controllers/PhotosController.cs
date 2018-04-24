using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PhotoGallery.Api.Services.Repositories;
using PhotoGallery.Data;
using PhotoGallery.Dto;

namespace PhotoGallery.Api.Controllers
{
    [Produces("application/json")]
    [Route("api/[controller]")]
    [Authorize]
    public class PhotosController : Controller
    {
        private readonly IPhotoRepository repository;
        private readonly IHostingEnvironment environment;

        public PhotosController(IPhotoRepository repository, IHostingEnvironment environment)
        {
            this.repository = repository;
            this.environment = environment;
        }
        // GET: api/Photos
        [HttpGet]
        public async Task<IActionResult> GetAsync()
        {
            var ownerId = User.Claims.FirstOrDefault(c => c.Type == "sub").Value;
            //var photos = await repository.GetPhotosAsync();
            var photos = await repository.GetPhotosAsync(ownerId);
            var photoDto = Mapper.Map<IEnumerable<PhotoDto>>(photos);
            return Ok(photoDto);
        }
        // GET: api/Photos/5
        [HttpGet("{id}", Name = "Get")]
        public async Task<IActionResult> GetAsync(Guid id)
        {
            var ownerId = User.Claims.FirstOrDefault(c => c.Type == "sub").Value;
            if (!await repository.IsOwnersPhoto(id, ownerId))
                return Forbid();
            if (id == null) return BadRequest();
            var photo = await repository.GetPhotoAsync(id);
            if (photo == null) return NotFound();
            var photoDto = Mapper.Map<PhotoDto>(photo);
            return Ok(photoDto);
        }

        // POST: api/Photos
        [HttpPost]
        public async Task<IActionResult> PostAsync([FromBody]PhotoCreateDto photoCreateDto)
        {
            if (photoCreateDto == null) return BadRequest();

            if (!ModelState.IsValid)
                return UnprocessableEntity(ModelState);

            var photo = Mapper.Map<Photo>(photoCreateDto);

            var webRootPath = environment.WebRootPath;

            var fileName = $"{Guid.NewGuid().ToString()}.jpg";

            var filePath = Path.Combine($"{webRootPath}/photos/{fileName}");

            await System.IO.File.WriteAllBytesAsync(filePath, photoCreateDto.Bytes);

            photo.FileName = fileName;

            await repository.AddPhotoAsync(photo);

            //var photoDto = Mapper.Map<PhotoDto>(photo);

            var photoDto = new PhotoDto()
            {
                Id = photo.Id,
                Title = photo.Title,
                FileName = photo.FileName
            };

            return CreatedAtRoute("Get", new { id = photoDto.Id }, photoDto);
        }

        // PUT: api/Photos/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutAsync(Guid id, [FromBody]PhotoUpdateDto photoUpdateDto)
        {
            var ownerId = User.Claims.FirstOrDefault(c => c.Type == "sub").Value;

            if (!await repository.IsOwnersPhoto(id, ownerId))
                return Forbid();

            if (photoUpdateDto == null) return BadRequest();

            if (!ModelState.IsValid)
                return UnprocessableEntity(ModelState);

            var photo = await repository.GetPhotoAsync(id);

            if (photo == null) return NotFound();

            Mapper.Map(photoUpdateDto, photo);

            await repository.UpdatePhotoAsync(photo);

            return NoContent();
        }

        // DELETE: api/ApiWithActions/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteAsync(Guid id)
        {
            if (id == null) return BadRequest();

            var ownerId = User.Claims.FirstOrDefault(c => c.Type == "sub").Value;

            if (!await repository.IsOwnersPhoto(id, ownerId))
                return Forbid();

            var photo = await repository.GetPhotoAsync(id);

            await repository.DeletePhoto(photo);

            return NoContent();
        }
    }
}
