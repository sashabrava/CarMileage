using CarMileage.Models;
using CarMileage.Data;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;

namespace CarMileage.Controllers
{
    public class DataManageController : Controller
    {
        private CarMileageContext db;
        public DataManageController()
        {
            this.db = new CarMileageContext();
        }

        [HttpGet]
        [Authorize]
        public IActionResult AddCar()
        {
            return View();
        }

        [HttpPost]
        [Authorize]
        public IActionResult AddCar(Car car)
        {
            //Request.Form.Keys
            if (ModelState.IsValid)
            {
                User user = db.Users.Where(x => x.Email == User.Identity.Name).First();
                car.Owner = user;
                this.db.Cars.Add(car);
                this.db.SaveChanges();
                return this.RedirectToAction("Index", "Home");
            }
            return new EmptyResult();
        }

        [HttpGet]
        [Authorize]
        public IActionResult EditCar(int carID)
        {
            Car car = this.db.Cars.Where(x => x.Id == carID).FirstOrDefault();
            User user = db.Users.Where(x => x.Email == User.Identity.Name).First();
            if (car == null)
                return NotFound();
            if (HasAdminRights(User))
            {
                ViewBag.Users = new SelectList(db.Users.ToList(), "Id", "Email");
                ViewBag.HasAdminRights = true;
                return View(car);
            }
            if (car.Owner == user)
                return View(car);
            else return StatusCode(403);
        }

        [HttpPost]
        [Authorize]
        public IActionResult EditCar(Car car)
        {
            this.db.Cars.Update(car);
            this.db.SaveChanges();
            return this.RedirectToAction("Index", "Home");
        }

        [HttpGet]
        [Authorize]
        public IActionResult ListCars()
        {
            if (HasAdminRights(User))
            {
                ViewBag.HasAdminRights = true;
                ViewBag.AdminMessage = "You are viewing this page as administrator";
                return View(this.db.Cars.Include(e => e.Owner).ToList());
            }
            else
            {
                User user = db.Users.Where(x => x.Email == User.Identity.Name).First();
                return View(this.db.Cars.Where(x => x.Owner == user).ToList());
            }

        }
        [HttpGet]
        public IActionResult AddMileage()
        {

            List<Car> cars = this.db.Cars.ToList();
            var selectList = from c in cars
                             select new SelectListItem
                             {
                                 Value = c.Id.ToString(),
                                 Text = String.Format("{0} {1} {2}", c.Manufacturer, c.Model, c.VIN)
                             };
            ViewBag.Cars = new SelectList(selectList, "Value", "Text");
            return View();
        }
        [HttpPost]
        public IActionResult AddMileage(Mileage mileage)
        {
            //Request.Form.Keys
            if (ModelState.IsValid)
            {
                this.db.Mileages.Add(mileage);
                this.db.SaveChanges();

                return this.RedirectToAction("Index", "Home");
            }
            return new EmptyResult();
        }
        private bool HasAdminRights(ClaimsPrincipal User)
        {
            string role = ((ClaimsIdentity)User.Identity).Claims
                 .Where(c => c.Type == ClaimTypes.Role)
                 .Select(c => c.Value).FirstOrDefault();
            if (role == "admin")
                return true;
            return false;
        }
    }
}