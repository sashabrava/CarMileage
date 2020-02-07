using CarMileage.Models;
using Microsoft.EntityFrameworkCore;
namespace CarMileage.Data
{
    public class CarMileageContext : DbContext
    {
        public DbSet<Car> Cars { get; set; }
        public DbSet<Mileage> Mileages { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<Role> Roles { get; set; }
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlite(@"Data source=data.db");
        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Mileage>().HasOne(m => m.Car).WithMany(m => m.Mileages).HasForeignKey(m => m.CarID);
            modelBuilder.Entity<User>().HasOne(m => m.Role).WithMany(m => m.Users).HasForeignKey(m => m.RoleId);
            modelBuilder.Entity<Car>().HasOne(m => m.Owner).WithMany().HasForeignKey(m => m.OwnerId);

        }
    }

}