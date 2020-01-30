using CarMileage.Models;
using CarMileage.Data;
namespace CarMileage.Controllers
{
    public class DataManageController
    {
        private CarMileageContext db;
                  public DataManageController()
        {
            this.db = new CarMileageContext();
        }
        
    }
}