using CarMileage.Models;
using Microsoft.EntityFrameworkCore;
namespace CarMileage.Data
{
    public class CarMileageContext :DbContext
    {
                public DbSet<Car> Cars { get; set; }
        public DbSet<Mileage> Mileages { get; set; }
                protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlite(@"Data source=data.db");
        }
    }
    
}