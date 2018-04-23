using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace PhotoGallery.Dto
{
    public class PhotoUpdateDto
    {
        [Required, MaxLength(150)]
        public string Title { get; set; }
    }
}
