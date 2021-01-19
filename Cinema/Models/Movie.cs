using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace Cinema.Models
{
    public class Movie
    {
        [Required]
        [Key, Column(Order = 1)]
        public string Title { get; set; }

        [Required]
        [Key, Column(Order = 2)]
        public System.DateTime DateMovie { get; set; }

        [Required]
        public string Category { get; set; }

        [Required]
        public int Price { get; set; }

        [Required]
        [Key, Column(Order = 3)]
        //[StringLength(1, MinimumLength = 1, ErrorMessage = "Stage Must Be Between 1 Characters")]
        public string Stage { get; set; }

        [Required]
        [DefaultValue(18)]
        public int Limit { get; set; }

    }
}