using CarMileage.Models;
using CarMileage.Data;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;

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
        public IActionResult AddCar()
        {
            return View();
        }

        [HttpPost]
        public IActionResult AddCar(Car car)
        {
            //Request.Form.Keys
            if (ModelState.IsValid)
            {
                this.db.Cars.Add(car);
                this.db.SaveChanges();
                return this.RedirectToAction("Index");
            }
            return new EmptyResult();
        }
        [HttpGet]
        public IActionResult EditCar(int carID)
        {
            if (carID > 0)
                return View(this.db.Cars.Where(x => x.Id == carID).First());
            return new EmptyResult();
        }
        [HttpPost]
        public IActionResult EditCar(Car car)
        {
            this.db.Cars.Update(car);
            this.db.SaveChanges();
            return this.RedirectToAction("Index", "Home");
        }

        [HttpGet]
        public IActionResult ListCars()
        {

            return View(this.db.Cars.ToList());
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

                return this.RedirectToAction("Index");
            }
            return new EmptyResult();
        }
    }
}