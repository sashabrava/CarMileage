using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;

namespace CarMileage.Controllers
{
    public class AccountController : Controller
    {
            public ActionResult Login()
    {
        return View();
    }

[Authorize]
    public ActionResult Logout()
    {
        return View();
    }
        
    }
}