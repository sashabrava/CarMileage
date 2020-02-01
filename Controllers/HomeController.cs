using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using CarMileage.Models;
using CarMileage.Data;
using Microsoft.AspNetCore.Http;
using System.IO;
using System.Text;
using System.Xml;
using Microsoft.AspNetCore.Authorization;

namespace CarMileage.Controllers
{
    public static class FileExtensions
    {

        public static /*List<string>*/StringBuilder ReadAsList(this IFormFile file)
        {
            var result = new StringBuilder();
            using (var reader = new StreamReader(file.OpenReadStream()))
            {
                while (reader.Peek() >= 0)
                    result.AppendLine(reader.ReadLine());
            }
            return result;
        }
    }
    public class HomeController : Controller
    {

        private CarMileageContext db;
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
            this.db = new CarMileageContext();
        }

        public IActionResult Index()
        {
            return View();
        }


        public IActionResult Privacy()
        {
            return View();
        }

        public IActionResult UploadGPX(IFormFile uploadedFile, String carID)
        {

            if (Request.Method == "GET")
            {
                ViewBag.Cars = this.db.Cars.ToList();
                return View();
            }

            else if (Request.Method == "POST")
            {
                XmlDocument doc = new XmlDocument();
                doc.LoadXml(uploadedFile.ReadAsList().ToString());
                //Convert.ToDateTime(doc.DocumentElement.GetElementsByTagName("time")[0].InnerXml);
                var result = doc.DocumentElement.GetElementsByTagName("trkpt");
                Dictionary<string, double> lastValues =
                    new Dictionary<string, double>();
                lastValues.Add("lat", 0);
                lastValues.Add("lon", 0);
                double distance = 0;
                foreach (XmlNode item in result)
                {
                    if (lastValues["lat"] != 0 && lastValues["lon"] != 0)
                        distance += /*GetDistanceBetweenPoints*/GetGPS(lastValues["lat"], lastValues["lon"], Convert.ToDouble(item.Attributes["lat"].Value), Convert.ToDouble(item.Attributes["lon"].Value));
                    lastValues["lat"] = Convert.ToDouble(item.Attributes["lat"].Value);
                    lastValues["lon"] = Convert.ToDouble(item.Attributes["lon"].Value);

                    //Console.WriteLine(item.Attributes["lat"].Value);
                    //Console.WriteLine("Department Name - " + item.Value);
                }
                Console.WriteLine(distance.ToString());
                Mileage mileage = new Mileage();
                mileage.Distance = Convert.ToInt32(distance / 1000);
                mileage.Date = Convert.ToDateTime(doc.DocumentElement.GetElementsByTagName("time")[0].InnerXml);
                mileage.CarID = Convert.ToInt32(carID);
                this.db.Mileages.Add(mileage);
                this.db.SaveChanges();
                return this.RedirectToAction("Index");
            }
            return new EmptyResult();
        }
        public IActionResult EditCar(int carID, Car car)
        {
            if (Request.Method == "GET")
            {
                 if (carID > 0)
                    return View(this.db.Cars.Where(x => x.Id == carID).First());
            }
            if (Request.Method == "POST")
            {
                car.Id = carID;
                this.db.Cars.Update(car);
                this.db.SaveChanges();
                return this.RedirectToAction("Index");
            }

            return new EmptyResult();
        }
        public IActionResult CarList()
        {

            return View(this.db.Cars.ToList());
        }
        public IActionResult AddCar(Car car)
        {
            if (Request.Method == "GET"){
                return View();
            }
            else if (Request.Method == "POST")
            {
                //Request.Form.Keys
                if (ModelState.IsValid)
                {
                    this.db.Cars.Add(car);
                    this.db.SaveChanges();

                    return this.RedirectToAction("Index");
                }
            }

            return new EmptyResult();
        }
        public IActionResult AddMileage(Mileage mileage)
        {
            /* List<System.Web.Mvc.SelectListItem> Cars = this.db.Cars
             .Select( c => new System.Web.Mvc.SelectListItem{Text=c.Id.ToString(), Value=c.VIN})
             .ToList();
             ViewBag.Cars = Cars;*/

            //List<Car> CarList = this.db.Cars.ToList();
            //ViewData["Cars"] = new System.Web.Mvc.SelectList(this.db.Cars.ToList(),"Id","VIN");
            if (Request.Method == "GET")
            {
                ViewBag.Cars = this.db.Cars.ToList();
                return View();
            }
            else if (Request.Method == "POST")
            {
                //Request.Form.Keys
                if (ModelState.IsValid)
                {
                    this.db.Mileages.Add(mileage);
                    this.db.SaveChanges();

                    return this.RedirectToAction("Index");
                }

            }
            //return View(this.db.Cars.ToList());
            return new EmptyResult();
        }
        /*public IActionResult AddCar (HttpContext context) {
            return new EmptyResult();
        }*/
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
        public double GetGPS(double lat1, double lon1, double lat2, double lon2)
        {
            //double distance = 0;
            int radius = 6378137;
            double DE2RA = 0.01745329252;
            lat1 *= DE2RA;
            lon1 *= DE2RA;
            lat2 *= DE2RA;
            lon2 *= DE2RA;
            double d = Math.Sin(lat1) * Math.Sin(lat2) + Math.Cos(lat1) * Math.Cos(lat2) * Math.Cos(lon1 - lon2);


            return radius * Math.Acos(d);
        }
    }
}
