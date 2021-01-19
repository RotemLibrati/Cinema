using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Cinema.Models
{
    public class GalleryMovie
    {
        [Required]
        [Key]
        public string MovieName { get; set; }
    }
}