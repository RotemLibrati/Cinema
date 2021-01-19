using Cinema.Dal;
using Cinema.Models;
using Cinema.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Cinema.Controllers
{
    public class TicketController : Controller
    {
        public ActionResult InviteMovie(string Title, DateTime Date, string StageName, int Price)
        {

            MovieTicketViewModel mtvm = new MovieTicketViewModel();
            mtvm.StringMovie = new List<string>();
            List<Ticket> ticket = new List<Ticket>();
            Ticket temp = new Ticket();
            temp.MovieName = Title;
            temp.Price = Price;
            temp.StageName = StageName;
            temp.UserName = "";
            temp.DateMovie = Date;
            
            if (User.Identity.IsAuthenticated)
                temp.UserName = User.Identity.Name.ToString();
            ticket.Add(temp);
            TicketDal dal = new TicketDal();

            bool flag = true;
            List<Ticket> t = new List<Ticket>();
            try
                {
                    t = (from x in dal.Tickets where x.MovieName.Equals(Title) select x).ToList<Ticket>();
                }
                catch
                {
                flag = false;
                }
            if (flag) { 
                List<Ticket> temp2 = new List<Ticket>();
                    for (int i = 0; i < t.Count; i++)
                    {
                        if (t[i].DateMovie == Date && t[i].StageName == StageName)
                            temp2.Add(t[i]);
                    }
                    mtvm.tickets = temp2;
                    mtvm.StringMovie.Add(Title);
                    mtvm.StringMovie.Add(StageName);
                    mtvm.StringMovie.Add(Price.ToString());
                    mtvm.DateMovie = Date;
                
                }
            else
            {
                mtvm.StringMovie.Add(Title);
                mtvm.StringMovie.Add(StageName);
                mtvm.StringMovie.Add(Price.ToString());
                mtvm.DateMovie = Date;
            }
            StageDal stageDal = new StageDal();
            List<Stage> s = new List<Stage>();
            s = (from x in stageDal.Stage where x.StageName.Equals(StageName) select x).ToList<Stage>();
            int number = s[0].NumberSeats;
            mtvm.StageSeat = number/12;






            return View("Seat", mtvm);
        }
        public ActionResult Payment(Ticket ticket, string Title, string StageName, int Price, DateTime DateMovie)
        {
            MovieTicketViewModel mtvm = new MovieTicketViewModel();
            TicketDal ticketDal = new TicketDal();
            mtvm.tickets = new List<Ticket>();
            mtvm.StringMovie = new List<string>();
            mtvm.Seats = "";
            string SeatTicket = null;
            try
            {
                SeatTicket = Request.Form["seat"].ToString();
            }
            catch
            {
                TempData["Message"] = "You don't choose a seat or You try to remove seat of another person";
                return RedirectToAction("ChooseMovie", "movie");
            }
            int count = SeatTicket.Split(',').Length;
            string[] temp = SeatTicket.Split(',');
            List<string> tempList = new List<string>(temp);
            List<Ticket> tickets = ticketDal.Tickets.ToList<Ticket>();
            foreach(Ticket t in tickets)
            {
                if(t.MovieName == Title && t.StageName == StageName && t.DateMovie == DateMovie)
                    for(int i=0;i<tempList.Count;i++)
                    {
                        if (tempList[i] == t.Seat)
                            tempList.RemoveAt(i);
                    }
            }
            string[] temp2 = new string[tempList.Count];
            for(int i=0;i<tempList.Count;i++)
            {
                temp2[i] = tempList[i];
            }
            for (int i = 0; i < temp2.Length; i++)
            {

                using (var databaseContext = new TicketDal())
                {

                    Ticket t = new Ticket();
                    t.MovieName = Title;
                    t.Price = Price;
                    t.StageName = StageName;
                    t.Seat = temp2[i];
                    t.DateMovie = DateMovie;
                    if (User.Identity.IsAuthenticated)
                    {
                        t.UserName = User.Identity.Name.ToString();
                    }
                    else
                        t.UserName = "Admin@gmail.com";
                    mtvm.tickets.Add(t);
                    mtvm.StringMovie.Add(t.MovieName);
                    mtvm.Seats += temp2[i] + ",";
                    databaseContext.Tickets.Add(t);
                    databaseContext.SaveChanges();                  
                }
                
            }

            if (mtvm.tickets.Count != 0)
            {
                mtvm.Sum = mtvm.tickets.Count * mtvm.tickets[0].Price;
                return View(mtvm);
            }
            else
            {
                TempData["Message"] = "You're trying to buy another person's seat or You dont choose a seat";
                return RedirectToAction("ChooseMovie", "movie");
            }
        }
        public ActionResult ConfirmPayment(string T, int P, DateTime Date, string Us,string St, string Seats )
        {
            string Month = Request.Form["Month"].ToString();
            string Year = Request.Form["Year"].ToString();
            int MonthTemp = 0;
            Int32.TryParse(Month, out MonthTemp);
            int YearTemp = 0;
            Int32.TryParse(Year, out YearTemp);
            DateTime Today = DateTime.Now;
            if (YearTemp >= Today.Year && MonthTemp >= Today.Month &&
                Request.Form["CardNumber"].ToString() != "" && Request.Form["CVV"].ToString() != "") {
                 TempData["Message"] = "Your ticket/s was/were purchased successfully";
                 return RedirectToAction("Index", "Account");
            }
            else 
            {
                using (var databaseContext = new TicketDal()) { 
                int count = Seats.Split(',').Length;
                string[] temp = Seats.Split(',');
                temp = temp.Take(temp.Count() - 1).ToArray();
                Ticket t = new Ticket();
                for(int i = 0; i < temp.Count(); i++) { 
                    t.MovieName = T;
                    t.Price = P;
                    t.StageName = St;
                    t.UserName = Us;
                    t.DateMovie = Date;
                    t.Seat = temp[i];
                    databaseContext.Tickets.Attach(t);
                    databaseContext.Tickets.Remove(t);
                    databaseContext.SaveChanges();
                    }
                }
                TempData["Message"] = "Your card is not valid or you didn't enter all details";
                return RedirectToAction("Index", "Account");
            }
        }
    }
}