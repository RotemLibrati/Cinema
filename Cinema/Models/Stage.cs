using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Cinema.Models
{
    public class Stage
    {
        [Key]
        [Required]
        public string StageName { get; set; }

        [Required]
        [Range(0,120)]
        public int NumberSeats { get; set; }
    }
}