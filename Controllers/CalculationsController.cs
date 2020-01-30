using Microsoft.AspNetCore.Mvc;
using CarMileage.Models;
using CarMileage.Data;
using System.Collections.Generic;
using System.Linq;
using System;


namespace CarMileage.Controllers
{

[Route("[controller]/[action]")]
    public class CalculationsController : Controller
    {
        
        private CarMileageContext db;

[Route("{carID}")]
                public IActionResult Calcs(int carID)
        {
            List <Mileage> mileages = this.db.Mileages.Where(s => s.CarID == carID).OrderBy(s => s.Date).ToList();
            Dictionary<string,object> dict = new Dictionary<string,object>();
  if (mileages.Where(s => s.OdometerMileage >0).Count() <2){
 dict["Message"] = "Car doesn't have enough info about odometer";
  }
 
  else{
dict["DayMileage"] = mileages.Where(s => s.Distance > 0);
dict["OdometerMileage"] = mileages.Where(s => s.OdometerMileage > 0);
dict["ChartDay"] = chartDailyMileage(mileages.Where(s => s.Distance > 0).ToList());
//calculateAverageDistance(mileages);
dict["chartOverall"] = chartOverallMileage(calculateAverageDistance(mileages));
  }
            return View(dict);
        }
                public CalculationsController()
        {
            this.db = new CarMileageContext();
        }

     public List<Dictionary<string,object>> chartOverallMileage(List<Mileage> allMileages){
List<Dictionary<string,object>> list = new List<Dictionary<string,object>>();
//Dictionary<string,List<object>> dict = new Dictionary<string,List<object>>();
//dict["x"] = new List<object>();
//dict["y"] = new List<object>();
foreach ( Mileage mileage in allMileages){
   Dictionary<string,object> dict = new Dictionary<string,object>();
   dict["x"] = mileage.Date.ToString();
   dict["y"] = mileage.OdometerMileage;
    list.Add(dict);

}
//JsonResult json = new JsonResult(dict);
//Console.WriteLine(json.Value);

return list;
     }

     public Dictionary<string,List<object>> chartDailyMileage(List<Mileage> allMileages){
Dictionary<string,List<object>> dict = new Dictionary<string,List<object>>();
dict["Labels"] = new List<object>();
dict["Data"] = new List<object>();


foreach ( Mileage mileage in allMileages){
dict["Labels"].Add(mileage.Date.ToString());
dict["Data"].Add(mileage.Distance);
}
//JsonResult json = new JsonResult(dict);
//Console.WriteLine(json.Value);

return dict;
     }
        public List<Mileage> calculateAverageDistance(List<Mileage> allMileages){
            //bool foundOdometerValue = false;
            Mileage previousMileage = new Mileage();
            List <Mileage> exactDayMileages = new List<Mileage>();
            foreach ( Mileage mileage in allMileages){
                if (previousMileage.OdometerMileage ==0 && mileage.OdometerMileage == 0)
                    continue;
                if (previousMileage.OdometerMileage == 0 && mileage.OdometerMileage > 0){
                    previousMileage = mileage;
                    continue;
                }
                if (mileage.Distance > 0){
                    exactDayMileages.Add(mileage);                    
                }
                if (mileage.OdometerMileage > 0 && mileage.OdometerMileage > previousMileage.OdometerMileage){
                    //means that we have full period for odometr prediction
                    int dailyMileageKnown = 0;
                    foreach ( Mileage mileagePerDay in exactDayMileages)
                        dailyMileageKnown += mileagePerDay.Distance;
                    int daysBetween = (mileage.Date - previousMileage.Date).Days;
                    int mileageBetween = mileage.OdometerMileage - previousMileage.OdometerMileage - dailyMileageKnown;
                    double dailyMileage = mileageBetween/daysBetween;
                    foreach ( Mileage mileagePerDay in exactDayMileages){
                        if (mileagePerDay.OdometerMileage == 0)
                            {
                                int daysFromPreviousMileage = (mileagePerDay.Date - previousMileage.Date).Days;
                                int mileagePredicted = previousMileage.OdometerMileage + (int)(dailyMileage * daysFromPreviousMileage + mileagePerDay.Distance);
                                 allMileages.First(x => x.Id == mileagePerDay.Id).OdometerMileage = mileagePredicted;
                            }
                }
                exactDayMileages = new List<Mileage>();
                previousMileage = mileage;

                
            }
        }
        return allMileages;
        }
    }
}