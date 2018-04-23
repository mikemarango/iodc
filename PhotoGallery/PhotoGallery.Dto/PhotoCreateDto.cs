using System;
using System.ComponentModel.DataAnnotations;

namespace PhotoGallery.Dto
{
    public class PhotoCreateDto
    {
        [Required, MaxLength(150)]
        public string Title { get; set; }
        [Required]
        public byte[] Bytes { get; set; }
    }
}
