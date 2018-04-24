using PhotoGallery.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PhotoGallery.Web.Models
{
    public class OrderPhotoViewModel
    {
        public string Address { get; set; } = string.Empty;

        public OrderPhotoViewModel(string address)
        {
            Address = address;
        }
    }
}
