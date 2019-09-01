using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using IdentityModel.Client;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using Newtonsoft.Json;
using PhotoGallery.Data;
using PhotoGallery.Dto;
using PhotoGallery.Web.Models;
using PhotoGallery.Web.Services;

namespace PhotoGallery.Web.Controllers
{
    [Authorize]
    public class PhotosController : Controller
    {
        private readonly PhotoService photoService;

        public PhotosController(PhotoService photoService)
        {
            this.photoService = photoService;
        }
        // GET: Photos
        public async Task<IActionResult> Index()
        {
            await WriteOutIdentityInformation();
            var photos = await photoService.GetPhotosAsync();

            if (photos == null)
            {
                return RedirectToAction("AccessDenied", "Account");
            }

            var photoViewModel = new PhotosViewModel()
            {
                Photos = photos
            };

            return View(photoViewModel);
        }

        // GET: Photos/Details/5
        public ActionResult Details(Guid id)
        {
            return View();
        }

        // GET: Photos/Create
        [Authorize(Roles = "PayingUser")]
        public ActionResult Create()
        {
            return View();
        }

        // POST: Photos/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "PayingUser")]
        public async Task<IActionResult> Create(CreatePhotoViewModel createPhotoViewModel)
        {
            if (!ModelState.IsValid)
            {
                return View();
            }

            var photoCreateDto = new PhotoCreateDto()
            {
                Title = createPhotoViewModel.Title,
            };

            if (createPhotoViewModel.Files.First().Length > 0)
            {
                using (var fileStream = createPhotoViewModel.Files.First().OpenReadStream())
                {
                    using (var memoryStream = new MemoryStream())
                    {
                        createPhotoViewModel.Files.First().CopyTo(memoryStream);
                        photoCreateDto.Bytes = memoryStream.ToArray();
                    }
                }
            }

            await photoService.AddPhotoAsync(photoCreateDto);

            return RedirectToAction("Index");
        }

        // GET: Photos/Edit/5
        public async Task<IActionResult> Edit(Guid id)
        {
            if (id == null)
            {
                return View();
            }
            var photo = await photoService.GetPhotoAsync(id);

            if (photo == null)
                return RedirectToAction("AccessDenied", "Account");

            var editPhotoViewModel = new EditPhotoViewModel()
            {
                Id = photo.Id,
                Title = photo.Title
            };
            return View(editPhotoViewModel);
        }

        // POST: Photos/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditAsync(EditPhotoViewModel editPhotoViewModel)
        {
            if (!ModelState.IsValid)
            {
                return View();
            }

            var photo = new Photo()
            {
                Id = editPhotoViewModel.Id,
                Title = editPhotoViewModel.Title
            };
            await photoService.UpdatePhotoAsync(photo.Id, photo);

            return RedirectToAction("Index");
        }

        //// GET: Photos/Delete/5
        //public ActionResult Delete(Guid id)
        //{
        //    return View();
        //}

        // POST: Photos/Delete/5
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(Guid id)
        {
            if (id == null) return View();
            
            await photoService.DeletePhotoAsync(id);

            return RedirectToAction("Index");
        }

        public async Task Logout()
        {
            await HttpContext.SignOutAsync("Cookies");
            await HttpContext.SignOutAsync("oidc");
        }

        [Authorize(Roles = "PayingUser")]
        public async Task<IActionResult> Order()
        {
            var address = await photoService.GetAddressAsync();
            return View(new OrderPhotoViewModel(address));
        }

        public async Task WriteOutIdentityInformation()
        {
            var identityToken = await HttpContext.GetTokenAsync(OpenIdConnectParameterNames.IdToken);
            Debug.WriteLine($"IdentityToken: {identityToken}");
            foreach (var claim in User.Claims)
            {
                Debug.WriteLine($"Claim type: {claim.Type}, claim value: {claim.Value}");
            }
        }
    }
}