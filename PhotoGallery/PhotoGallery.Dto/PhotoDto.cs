using System;
using System.Collections.Generic;
using System.Text;

namespace PhotoGallery.Dto
{
    public class PhotoDto
    {
        public Guid Id { get; set; }
        public string Title { get; set; }
        public string FileName { get; set; } = string.Empty;
    }
}
