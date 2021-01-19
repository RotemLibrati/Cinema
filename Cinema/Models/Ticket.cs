using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace Cinema.Models
{
    public class Ticket
    {  
        [Required]
        public string MovieName { get; set; }


        [Required]
        [Key, Column(Order = 1)]
        public DateTime DateMovie { get; set; }


        [Required]
        [Key, Column(Order = 2)]
        public string StageName { get; set; }

        [Required]
        [Key, Column(Order = 3)]
        public string Seat { get; set; }

       

        [Required]
        public int Price { get; set; }
        
        

        [Required]
        public string UserName { get; set; }


    }
}