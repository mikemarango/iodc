using PhotoGallery.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PhotoGallery.Web.Models
{
    public class PhotosViewModel
    {
        public IEnumerable<Photo> Photos { get; set; } = new List<Photo>();
    }
}
