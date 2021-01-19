using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Cinema.Models;
namespace Cinema.ViewModel
{
    public class MovieViewModel
    {
        public Movie movie { get; set; }

        public List<Movie> movies { get; set; }
        public List<GalleryMovie> galleryMovies { get; set; }
    }
}