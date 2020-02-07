using System.Collections.Generic;
namespace CarMileage.Models
{
    public class Car
    {
        public int Id { get; set; }
        public string Manufacturer { get; set; }
        public string Model { get; set; }

        public string VIN { get; set; }

        public User Owner { get; set; }
        public int OwnerId { get; set; }
        public List<Mileage> Mileages { get; set; }
    }
}
