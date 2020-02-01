using CarMileage.Models;
using CarMileage.Data;
using Microsoft.AspNetCore.Mvc;
using System.Xml;
using System.Collections.Generic;
using System.Linq;
using System;
using Microsoft.AspNetCore.Http;
using System.IO;
using System.Text;

namespace CarMileage.Controllers
{
    public static class FileExtensions
    {

        public static StringBuilder ReadAsList(this IFormFile file)
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
    public class GPXController : Controller
    {
        private CarMileageContext db;
        public GPXController()
        {
            this.db = new CarMileageContext();
        }

        [HttpGet]
        public IActionResult UploadGPX()
        {
            ViewBag.Cars = this.db.Cars.ToList();
            return View();

        }

        [HttpPost]
        public IActionResult UploadGPX(IFormFile uploadedFile, String carID)
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