using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace PhotoGallery.Web.Models
{
    public class CreatePhotoViewModel
    {
        [Display(Name = "Photo Title")]
        public string Title { get; set; }
        public List<IFormFile> Files { get; set; } = new List<IFormFile>();
    }
}
