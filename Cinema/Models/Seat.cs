using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
namespace Cinema.Models
{
    public class Seat
    {
        [Required]
        public string Place { get; set; }

        [Required]
        public bool Available { get; set; }

        [Required]
        public string Stage { get; set; }
    }
}