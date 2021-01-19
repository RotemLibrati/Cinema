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
    public class StageController : Controller
    {

        public ActionResult CreateStage()
        {
            return View();
        }
        [HttpPost]
        public ActionResult SaveStage(Stage stage)
        {
            if (ModelState.IsValid)
            {
                //create database context using Entity framework 
                using (var databaseContext = new StageDal())
                {
                    Stage s = new Stage();
                    s.StageName = stage.StageName;
                    s.NumberSeats = stage.NumberSeats;
                    databaseContext.Stage.Add(s);
                    try
                    {
                        databaseContext.SaveChanges();
                }
                    catch
                {
                    Response.Write("<script>alert('The StageName you selected is alreay exist')</script>");
                    return View("CreateStage", stage);
                }
            }

                ViewBag.Message = "User Details Saved";
                return RedirectToAction("Index", "Account");
            }
            else
            {
                return View("CreateStage", stage);
            }
        }
    }
}