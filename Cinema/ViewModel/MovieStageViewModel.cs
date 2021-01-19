using Cinema.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Cinema.ViewModel
{
    public class MovieStageViewModel
    {
        public Movie movie { get; set; }
        public List<Movie> movies { get; set; }
        public Stage stage { get; set; }
        public List<Stage> stages { get; set; }
    }
}