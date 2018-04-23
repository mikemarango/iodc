using System;
using System.Collections.Generic;
using System.Text;

namespace PhotoGallery.Data
{
    public class Photo
    {
        public Guid Id { get; set; }
        public string Title { get; set; }
        public string FileName { get; set; }
        public string OwnerId { get; set; }
    }
}
