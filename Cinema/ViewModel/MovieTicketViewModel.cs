using Cinema.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Cinema.ViewModel
{
    public class MovieTicketViewModel
    {
        public Movie movie { get; set; }
        public List<Movie> UserList { get; set; }
        public Ticket ticket { get; set; }
        public List<Ticket> tickets { get; set; }
        public List<string> StringMovie { get; set; }
        public int StageSeat { get; set; }
        public DateTime DateMovie { get; set; } 
        public string Seats { get; set; }
        public int Sum { get; set; }
        public IEnumerable<SelectListItem> categoryList { get; set; }
    }
}