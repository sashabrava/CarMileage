using System;

namespace CarMileage.Models
{
    public class Mileage
    {
         public int Id { get; set; }
        public Car Car { get; set; }
        public int CarID {get;set;}       
        public DateTime Date { get; set; } 

        public int Distance {get;set;}

        public int OdometerMileage {get;set;}
    }
}