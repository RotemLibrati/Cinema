using Cinema.Dal;
using Cinema.Models;
using Cinema.ViewModel;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Cinema.Controllers
{
    public class MovieController : Controller
    {
        
        public ActionResult Index()
        {
            return View();
        }
        
        
        [HttpPost]
        public ActionResult SaveMovieDetails(Movie movieDetails)
        {
            if (ModelState.IsValid)
            {
                //create database context using Entity framework 
                using (var databaseContext = new MovieDal())
                {
                    StageDal stageDal = new StageDal();
                    List<Stage> stages = stageDal.Stage.ToList<Stage>();
                    MovieDal movieDal = new MovieDal();
                    List<Movie> movies = null;
                    List<Movie> movieStage = movieDal.Movie.ToList<Movie>();
                    GalleryMovieDal galleryMovieDal = new GalleryMovieDal();
                    List<GalleryMovie> galleryMovies = galleryMovieDal.GalleryMovie.ToList<GalleryMovie>();
                    int countMovie = 0;
                    bool exist = false;
                    foreach (GalleryMovie mg in galleryMovies)
                    {
                        if(mg.MovieName.Equals(movieDetails.Title))
                        {
                            exist = true;
                        }
                    }
                    if (!exist && galleryMovies.Count >0)
                    {
                        Response.Write("<script>alert('This movie don't exist in the gallery')</script>");
                        return View("Create", movieDetails);
                    }
                    foreach (Movie mo in movieStage)
                    {
                        if (mo.Stage == movieDetails.Stage &&
                            mo.DateMovie.Date.ToString("dd/mm/yyy") == movieDetails.DateMovie.ToString("dd/mm/yyy"))
                            countMovie++;
                        if (countMovie == 4)
                        {
                            Response.Write("<script>alert('The stage is full for this date')</script>");
                            return View("Create", movieDetails);
                        }

                    }
                    foreach(Stage s in stages)//Check if the stage is available in date and time that the admin create
                    {
                        
                        if (s.StageName == movieDetails.Stage)
                        {
                            movies = (from x in movieDal.Movie where x.Stage.Equals(s.StageName) select x).ToList<Movie>();
                            foreach (Movie m in movies)
                            {
                                if(m.DateMovie.Day == movieDetails.DateMovie.Day &&
                                    m.DateMovie.Month == movieDetails.DateMovie.Month &&
                                    m.DateMovie.Year == movieDetails.DateMovie.Year)
                                {
                                    if(m.DateMovie.Hour > movieDetails.DateMovie.Hour)
                                    {
                                        if(m.DateMovie.Hour - movieDetails.DateMovie.Hour < 3)
                                        {
                                            Response.Write("<script>alert('At the time you choosed, a movie is already being shown in this stage')</script>");
                                            return View("Create", movieDetails);
                                        }
                                    }
                                    else if(m.DateMovie.Hour < movieDetails.DateMovie.Hour)
                                    {
                                        if(movieDetails.DateMovie.Hour - m.DateMovie.Hour < 3) { 
                                            Response.Write("<script>alert('At the time you choosed, a movie is already being shown in this stage')</script>");
                                            return View("Create", movieDetails);
                                        }
                                    }
                                }
                            }
                        }
                    }
                    bool flag = false;
                    foreach(Stage s in stages)
                    {
                        if (s.StageName == movieDetails.Stage)
                        {
                            flag = true;
                            break;
                        }
                    }
                    if (flag)
                    {
                        Movie m = new Movie();
                        m.Title = movieDetails.Title;
                        m.DateMovie = movieDetails.DateMovie;
                        m.Category = movieDetails.Category;
                        m.Price = movieDetails.Price;

                        try
                        {
                            m.Stage = movieDetails.Stage;
                        }
                        catch
                        {
                            Response.Write("<script>alert('The stage you selected is not ligal')</script>");
                            return View("Create", movieDetails);
                        }

                        m.Limit = movieDetails.Limit;


                        databaseContext.Movie.Add(m);
                        try
                        {
                            databaseContext.SaveChanges();
                        }
                        catch
                        {
                            Response.Write("<script>alert('The Title you selected is alreay exist')</script>");
                            return View("Create", movieDetails);
                        }
                    }
                    else
                    {
                        Response.Write("<script>alert('You select stage that dont exist !')</script>");
                        return View("Create", movieDetails);
                    }
                }

                ViewBag.Message = "User Details Saved";
                return RedirectToAction("Index", "Account");
            }
            else
            {
                return View("Create", movieDetails);
            }
        }
        public ActionResult Seat()
        {
            
            return View();
        }

        public ActionResult Choose()
        {
            MovieStageViewModel msvm = new MovieStageViewModel();
            msvm.movies = new List<Movie>();
            MovieViewModel mvm = new MovieViewModel();
            MovieDal dal = new MovieDal();
            string Title = Request.Form["Title"].ToString();
            string DateMovie = Request.Form["DateMovie"].ToString();
            string Category = Request.Form["Category"].ToString();
            string Limit = Request.Form["Limit"].ToString();
            string Price = Request.Form["Price"].ToString();
            string newDateMovie = "";
            if (DateMovie != "")
            {
                string Year = DateMovie.Substring(0, 4);
                string Month = DateMovie.Substring(5, 2);
                string Day = DateMovie.Substring(8, 2);
                string time = DateMovie.Substring(11, 5);

                newDateMovie = Day + "/" + Month + "/" + Year + " " + time + ":00";
            }
            List<Movie> objMovie = null;
            List<Movie> temp = new List<Movie>();
            // search by title
            if (Title != "" && Category == "" && newDateMovie == "" && Price =="" && Limit=="") { 
                objMovie = (from x in dal.Movie where x.Title.Equals(Title) select x).ToList<Movie>();
            }
            // search by category
            else if (Title == "" && Category !=""  && Price == "" && Limit == "")// 
            {
                if(newDateMovie == "")
                    objMovie = (from x in dal.Movie where x.Category.Equals(Category) select x).ToList<Movie>();
                else
                {
                    mvm.movies = dal.Movie.ToList<Movie>();
                    DateTime t = DateTime.Parse(newDateMovie);
                    foreach (Movie m in mvm.movies)
                    {
                        if (t.Year == m.DateMovie.Year && t.Day == m.DateMovie.Day && t.Month == m.DateMovie.Month)
                        {
                            if (t.TimeOfDay < m.DateMovie.TimeOfDay)
                                if (m.Category == Category)
                                {
                                    temp.Add(m);
                                }
                        }

                    }
                    objMovie = temp;
                }
            }
            else if (Title == "" && Category == ""  && newDateMovie != "" && Price == "" && Limit == "")
            {
                mvm.movies = dal.Movie.ToList<Movie>();
                DateTime t =  DateTime.Parse(newDateMovie);
                foreach (Movie m in mvm.movies)
                {
                    if (t.Year == m.DateMovie.Year && t.Day == m.DateMovie.Day && t.Month == m.DateMovie.Month)
                    {
                        if(t.TimeOfDay < m.DateMovie.TimeOfDay)
                            temp.Add(m);
                    }
                        
                }
                objMovie = temp;
            }
            else if (Title != "" && Category == "" && newDateMovie != "" && Price == "" && Limit == "")//serach by date and title
            {
                mvm.movies = dal.Movie.ToList<Movie>();
                DateTime t = DateTime.Parse(newDateMovie);
                foreach (Movie m in mvm.movies)
                {
                    if (t.Year == m.DateMovie.Year && t.Day == m.DateMovie.Day && t.Month == m.DateMovie.Month)
                    {
                        if (t.TimeOfDay < m.DateMovie.TimeOfDay)
                            if (m.Title == Title) { 
                                temp.Add(m);
                            }
                    }

                }
                objMovie = temp;
            }
            else if (Title == "" && Category == "" && Limit !="" && Price=="")//search by limit and date/without date
            {
                int y = 0;
                Int32.TryParse(Limit, out y);
                if(newDateMovie == "")
                    objMovie = (from x in dal.Movie where x.Limit <= y select x).ToList<Movie>();
                else
                {
                    mvm.movies = dal.Movie.ToList<Movie>();
                    DateTime t = DateTime.Parse(newDateMovie);
                    foreach (Movie m in mvm.movies)
                    {
                        if (t.Year == m.DateMovie.Year && t.Day == m.DateMovie.Day && t.Month == m.DateMovie.Month)
                        {
                            if (t.TimeOfDay < m.DateMovie.TimeOfDay)
                                if (m.Limit == y)
                                {
                                    temp.Add(m);
                                }
                        }

                    }
                    objMovie = temp;
                }
            }
            else if (Title == "" && Category == "" && newDateMovie == "" && Limit != "" && Price != "")//search by limit
            {
                int lim = 0;
                Int32.TryParse(Limit, out lim);
                int pri = 0;
                Int32.TryParse(Price, out pri);
                objMovie = (from x in dal.Movie where x.Limit <= lim  &&  x.Price <= pri select x).ToList<Movie>();
            }
            else if (Title == "" && Category == "" && newDateMovie == "" && Limit == "" && Price != "")
            {
                int y = 0;
                Int32.TryParse(Price, out y);
                objMovie = (from x in dal.Movie where x.Price <= y select x).ToList<Movie>();
            }
            else if (Title == "" && Category == "" && newDateMovie == "" && Limit == "" && Price == "")
            {
                objMovie = dal.Movie.ToList<Movie>();
            }
            else if (Title == "" && Category == "" && newDateMovie != "" && Price != "" && Limit == "")//serach by date and title
            {
                mvm.movies = dal.Movie.ToList<Movie>();
                DateTime t = DateTime.Parse(newDateMovie);
                int y = 0;
                Int32.TryParse(Price, out y);
                foreach (Movie m in mvm.movies)
                {
                    if (t.Year == m.DateMovie.Year && t.Day == m.DateMovie.Day && t.Month == m.DateMovie.Month)
                    {
                        if (t.TimeOfDay < m.DateMovie.TimeOfDay)
                            if (m.Price < y)
                            {
                                temp.Add(m);
                            }
                    }

                }
                objMovie = temp;
            }
            else if (Title == "" && Category == "" && newDateMovie != "" && Price == "" && Limit != "")//serach by date and title
            {
                mvm.movies = dal.Movie.ToList<Movie>();
                DateTime t = DateTime.Parse(newDateMovie);
                int y = 0;
                Int32.TryParse(Limit, out y);
                foreach (Movie m in mvm.movies)
                {
                    if (t.Year == m.DateMovie.Year && t.Day == m.DateMovie.Day && t.Month == m.DateMovie.Month)
                    {
                        if (t.TimeOfDay < m.DateMovie.TimeOfDay)
                            if (m.Limit < y)
                            {
                                temp.Add(m);
                            }
                    }

                }
                objMovie = temp;
            }
            else
            {
                TempData["Message"] = "You are trying to search for inappropriate categories";
                return RedirectToAction("ChooseMovie", "Movie");
            }
            mvm.movies = objMovie;
            return View("ShowMovie", mvm);    

        }
        public ActionResult Create()
        {
            return View();
        }
        public ActionResult GalleryMovie()
        {
            return View();
        }
        public ActionResult DetailsMovies(string Title)
        {
            MovieDal dal = new MovieDal();
            MovieViewModel mvm = new MovieViewModel();
            List<Movie> movies = (from x in dal.Movie where x.Title.Equals(Title) select x).ToList<Movie>();
            mvm.movies = movies;
            ViewData["Title"] = Title;
            return View("DetailsMovie");
        }



        public ActionResult ShowSearch()
        {
            MovieDal dal = new MovieDal();
            MovieViewModel mvm = new MovieViewModel();
            try { 
            List<Movie> movies = dal.Movie.ToList<Movie>();
                mvm.movie = new Movie();
                mvm.movies = movies;
            }
            catch
            {
                Response.Write("<script>alert('The library of movies is empty')</script>");
            }

            CartDal cartDal = new CartDal();
            List<Cart> cart = cartDal.Cart.ToList<Cart>();
            using (var databaseContext = new CartDal())
            {
                foreach (Cart c in cart)
                {
                    if (DateTime.Now.Subtract(c.DateCreate).Minutes > 10)
                    {
                        databaseContext.Cart.Attach(c);
                        databaseContext.Cart.Remove(c);
                        databaseContext.SaveChanges();
                        var data = new TicketDal();
                        Ticket t = new Ticket();
                        t.MovieName = c.MovieName;
                        t.Price = c.Price;
                        t.DateMovie = c.DateMovie;
                        t.Seat = c.Seat;
                        t.UserName = c.UserName;
                        t.StageName = c.StageName;
                        data.Tickets.Attach(t);
                        data.Tickets.Remove(t);
                        data.SaveChanges();
                    }
                }
            }
            cart = cartDal.Cart.ToList<Cart>();
            return View("ChooseMovie", mvm);
        }
        public ActionResult Delete()
        {
            return View();
        }
        [HttpPost]
        public ActionResult DeleteRow(Movie movieDetails)
        {
            if (ModelState.IsValid)
            {
                //create database context using Entity framework 
                using (var databaseContext = new MovieDal())
                {
                    Movie m = new Movie();
                    try
                    {
                        m.Title = movieDetails.Title;
                    }
                    catch
                    {
                        Response.Write("<script>alert('The stage you selected is not ligal')</script>");
                        return View("Create", movieDetails);
                    }
                    m.DateMovie = movieDetails.DateMovie;
                    m.Category = movieDetails.Category;
                    m.Price = movieDetails.Price;
                    m.Stage = movieDetails.Stage;
                    m.Limit = movieDetails.Limit;
                    try
                    {
                        databaseContext.Movie.Attach(m);
                        databaseContext.Movie.Remove(m);
                        databaseContext.SaveChanges();
                    }
                    catch
                    {
                        Response.Write("<script>alert('The Title you selected is alreay exist')</script>");
                        return View("Create", movieDetails);
                    }
                }

                ViewBag.Message = "User Details Saved";
                return RedirectToAction("Index", "Account");
            }
            else
            {
                return View("Index", movieDetails);
            }
        }

        public ActionResult ChooseMovie()
        {
            return View();
        }


        public ActionResult UpdatePrice()
        {
            string t = Request.Form["Title"].ToString();
            string p = Request.Form["Price"].ToString();
            int pPrice ;
            Int32.TryParse(p, out pPrice);

            var databaseContext = new MovieDal();
            var query = from x in databaseContext.Movie where x.Title == t select x;
            var temp = databaseContext.Movie.FirstOrDefault(x => x.Title == t);
            int tempPrice = temp.Price;
            foreach(Movie y in query)
            {
                y.Price = pPrice;
            }
            try
            {
                databaseContext.SaveChanges();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
            if(pPrice < tempPrice)
            {
                string message = $"The movie {t} was reduced in price to {pPrice}₪ ";//message to user if has discount to ticket.
                Session["Message"] = message;
            }
            return RedirectToAction("Index", "Account");
        }
        public ActionResult UpdatePriceTransition()
        {
            return View();
        }

        public ActionResult PopularMovie()
        { 
            MovieViewModel mvm = new MovieViewModel();
            TicketDal dal = new TicketDal();
            MovieDal dalMovie = new MovieDal();
            List<Ticket> tickets = dal.Tickets.ToList<Ticket>();
            List<int> count = new List<int>(new int[9]);
            /*
             1.PrisonBreak
             2.BatMan
             3.Titanic
             4.Rambo
             5.Pokemon
             6.SuperMan
             7.SpiderMan
             8.ScaryMovie4
             9.Fast&Furious
             */
            int i;
            for (i = 0; i < count.Count; i++)
            {
                count[i] = 0;
            }
            foreach (Ticket t in tickets)
            {
                if (t.MovieName == "PrisonBreak")
                    count[0]++;
                else if (t.MovieName == "BatMan")
                    count[1]++;
                else if (t.MovieName == "Titanic")
                    count[2]++;
                else if (t.MovieName == "Rambo")
                    count[3]++;
                else if (t.MovieName == "Pokemon")
                    count[4]++;
                else if (t.MovieName == "SuperMan")
                    count[5]++;
                else if (t.MovieName == "SpiderMan")
                    count[6]++;
                else if (t.MovieName == "ScaryMovie4")
                    count[7]++;
                else if (t.MovieName == "Fast&Furious")
                    count[8]++;
            }
            int temp = 0, tempI=0;
            for(i=0;i<count.Count;i++)
            {
                if(count[i]>temp)
                {
                    temp = count[i];
                    tempI = i;
                }
            }
            string tempMovie;
            if (tempI == 0)
                tempMovie = "PrisonBreak";
            else if (tempI == 1)
                tempMovie = "BatMan";
            else if (tempI == 2)
                tempMovie = "Titanic";
            else if (tempI == 3)
                tempMovie = "Rambo";
            else if (tempI == 4)
                tempMovie = "Pokemon";
            else if (tempI == 5)
                tempMovie = "SuperMan";
            else if (tempI == 6)
                tempMovie = "SpiderMan";
            else if (tempI == 7)
                tempMovie = "ScaryMovie4";
            else if (tempI == 8)
                tempMovie = "Fast&Furious";
            else
                tempMovie = null;

            List<Movie> movies = (from x in dalMovie.Movie where x.Title.Equals(tempMovie) select x).ToList<Movie>();
            mvm.movies = movies;
            return View("ShowMovie", mvm);
        }
    }
    

}