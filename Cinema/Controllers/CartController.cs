using Cinema.Models;
using Cinema.Dal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Cinema.ViewModel;

namespace Cinema.Controllers
{
    public class CartController : Controller
    {
        public ActionResult AddToCart(string T, int P, DateTime Date, string Us, string St, string Seats)
        {
            CartDal cartDal = new CartDal();
            if (T != null)
            {
                
                int count = Seats.Split(',').Length;
                string[] temp = Seats.Split(',');
                temp = temp.Take(temp.Count() - 1).ToArray();
                List<string> tempList = new List<string>(temp);
                List<Cart> carts = cartDal.Cart.ToList<Cart>();
                foreach (Cart c in carts)
                {
                    if (c.MovieName == T && c.StageName == St && c.DateMovie == Date)
                        for (int i = 0; i < tempList.Count; i++)
                        {
                            if (tempList[i] == c.Seat)
                                tempList.RemoveAt(i);
                        }
                }
                string[] temp2 = new string[tempList.Count];
                for (int i = 0; i < tempList.Count; i++)
                {
                    temp2[i] = tempList[i];
                }
                for (int i = 0; i < temp2.Count(); i++)
                {
                    using (var databaseContext = new CartDal())
                    {
                        Cart c = new Cart();
                        c.MovieName = T;
                        c.Price = P;
                        c.StageName = St;
                        c.UserName = Us;
                        c.DateMovie = Date;
                        c.Seat = temp[i];
                        c.DateCreate = DateTime.Now;
                        databaseContext.Cart.Add(c);
                        databaseContext.SaveChanges();
                    }
                }
            }
            CartViewModel cvm = new CartViewModel();
            List<Cart> cart = cartDal.Cart.ToList<Cart>();
            using (var databaseContext = new CartDal())
            {
                foreach(Cart c in cart)
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
            cvm.carts = new List<Cart>();
            int sum = 0;
            foreach (Cart c in cart)
            {
                if (c.UserName == User.Identity.Name.ToString())
                {
                    cvm.carts.Add(c);
                    sum += c.Price;
                }
            }
            cvm.Sum = sum;
            return View("Cart", cvm);
        }
        public ActionResult ShowCart()
        {
            CartDal cartDal = new CartDal();
            CartViewModel cvm = new CartViewModel();
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
            cvm.carts = new List<Cart>();
            int sum = 0;
            foreach (Cart c in cart)
            {
                if (c.UserName == User.Identity.Name.ToString())
                {
                    cvm.carts.Add(c);
                    sum += c.Price;
                }
            }
            cvm.Sum = sum;
            return View(cvm);
        }
        public ActionResult PaymentCart(string name, int sum)
        {
            CartViewModel cvm = new CartViewModel();
            cvm.nameUser = name;
            cvm.Sum = sum;
            return View(cvm);
        }
        public ActionResult Confirm()
        {
            string name = User.Identity.Name.ToString();
            string Month = Request.Form["Month"].ToString();
            string Year = Request.Form["Year"].ToString();
            int MonthTemp = 0;
            Int32.TryParse(Month, out MonthTemp);
            int YearTemp = 0;
            Int32.TryParse(Year, out YearTemp);
            DateTime Today = DateTime.Now;
            if (YearTemp >= Today.Year && MonthTemp >= Today.Month &&
                Request.Form["CardNumber"].ToString() != "" && Request.Form["CVV"].ToString() != "")
            {
                CartDal cartDal = new CartDal();
                List<Cart> carts = cartDal.Cart.ToList<Cart>();
                foreach (Cart c in carts)
                {
                    if (c.UserName == name)
                    {
                        var data = new CartDal();
                        data.Cart.Attach(c);
                        data.Cart.Remove(c);
                        data.SaveChanges();
                    }
                }
                TempData["Message"] = "Your ticket/s was/were purchased successfully";
                return RedirectToAction("Index", "Account");
            }
            else
            {
                TempData["Message"] = "Your card is not valid";
                return RedirectToAction("Index", "Account");
            }
        }
    }
}