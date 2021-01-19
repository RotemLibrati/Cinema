using Cinema.Dal;
using Cinema.Models;
using Cinema.ViewModel;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Web.Mvc;
using System.Web.Security;

namespace Cinema.Controllers
{
    public class AccountController : Controller
    {

        // Return Home page.
        public ActionResult Index()
        {
            RegisterUserDal regDal = new RegisterUserDal();
            RegisterViewModel rvm = new RegisterViewModel();
            List<Register> accounts = regDal.RegisterUser.ToList<Register>();
            foreach (Register x in accounts)
                if (x.Email == User.Identity.Name.ToString())
                {
                    ViewData["Name"] = x.FirstName;
                    ViewData["Lname"] = x.LastName;
                }
            return View();
        }

        //Return Register view
        public ActionResult Register()
        {
            return View();
        }

        //The form's data in Register view is posted to this method. 
        //We have binded the Register View with Register ViewModel, so we can accept object of Register class as parameter.
        //This object contains all the values entered in the form by the user.
        [HttpPost]
        public ActionResult SaveRegisterDetails(Register registerDetails)
        {
            //We check if the model state is valid or not. We have used DataAnnotation attributes.
            //If any form value fails the DataAnnotation validation the model state becomes invalid.
            if (ModelState.IsValid)
            {
                //create database context using Entity framework 
                using (var databaseContext = new RegisterUserDal())
                {
                    //If the model state is valid i.e. the form values passed the validation then we are storing the User's details in DB.
                    Register reglog = new Register();

                    //Save all details in RegitserUser object

                    reglog.FirstName = registerDetails.FirstName;
                    reglog.LastName = registerDetails.LastName;
                    reglog.BirthDate = registerDetails.BirthDate;
                    reglog.Email = registerDetails.Email;
                    reglog.Password = registerDetails.Password;


                    //Calling the SaveDetails method which saves the details.

                    databaseContext.RegisterUser.Add(reglog);
                    try
                    {
                        databaseContext.SaveChanges();
                    }
                    catch
                    {
                        Response.Write("<script>alert('The email you selected is not available')</script>");
                        return View("Register", registerDetails);
                    }

                }

                ViewBag.Message = "User Details Saved";
                TempData["Name"] = registerDetails.FirstName;
                CartDal cartDal = new CartDal();
                CartViewModel cvm = new CartViewModel();
                List<Cart> cart = cartDal.Cart.ToList<Cart>();
                cvm.carts = new List<Cart>();
                foreach (Cart c in cart)
                {
                    if (c.UserName == User.Identity.Name.ToString())
                        cvm.carts.Add(c);
                }
                return RedirectToAction("Index", cvm);
            }
            else
            {

                //If the validation fails, we are returning the model object with errors to the view, which will display the error messages.
                return View("Register", registerDetails);
            }
        }


        public ActionResult Login()
        {
            return View();
        }

        //The login form is posted to this method.
        [HttpPost]
        public ActionResult Login(LoginViewModel model)
        {
            CartDal cartDal = new CartDal();
            List<Cart> cart = cartDal.Cart.ToList<Cart>();
            using (var databaseContext = new CartDal())
            {
                foreach (Cart c in cart)
                {
                    if (DateTime.Now.Subtract(c.DateCreate).Minutes > 1)
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
            //Checking the state of model passed as parameter.
            if (ModelState.IsValid)
            {

                //Validating the user, whether the user is valid or not.
                var isValidUser = IsValidUser(model);

                //If user is valid & present in database, we are redirecting it to Welcome page.
                if (isValidUser != null)
                {
                    
                    FormsAuthentication.SetAuthCookie(model.Email, false);
                    return RedirectToAction("Index");
                }
                else
                {
                    //If the username and password combination is not present in DB then error message is shown.
                    ModelState.AddModelError("Failure", "Wrong Username and password combination !");
                    return View();
                }
            }
            else
            {
                //If model state is not valid, the model with error message is returned to the View.
                return View(model);
            }
        }

        //function to check if User is valid or not
        public Register IsValidUser(LoginViewModel model)
        {
            using (var dataContext = new RegisterUserDal())
            {
                //Retireving the user details from DB based on username and password enetered by user.
                Register user = dataContext.RegisterUser.Where(query => query.Email.Equals(model.Email) && query.Password.Equals(model.Password)).SingleOrDefault();
                //If user is present, then true is returned.
                if (user == null)
                    return null;
                //If user is not present false is returned.
                else
                    return user;
            }
        }


        public ActionResult Logout()
        {
            FormsAuthentication.SignOut();
            Session.Abandon(); // it will clear the session at the end of request
            return RedirectToAction("Index");
        }
        public ActionResult Contact()
        {
            TempData["Message"] = "The message was forwarded to the admin";
            return View();
        }
    }
}