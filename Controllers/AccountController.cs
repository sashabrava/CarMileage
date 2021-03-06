using Microsoft.AspNetCore.Mvc;

using CarMileage.Models;
using CarMileage.Data;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CarMileage.Controllers
{
    public class AccountController : Controller
    {
        public CarMileageContext db;
        public AccountController()
        {
            this.db = new CarMileageContext();
        }
        public async Task<IActionResult> Login(LoginModel loginModel)

        {
            //Request.Form.Keys
            if (Request.Method == "GET")
            {
                return View();
            }
            else if (Request.Method == "POST")
            {
                if (ModelState.IsValid)
                {
                    User user = this.db.Users.Include("Role").FirstOrDefault(x => x.Email == loginModel.Email);
                    if (user != null && Security.checkPassword(loginModel.Password, user.Password))
                    {
                        await Authenticate(user);
                        return RedirectToAction("Index", "Home");
                    }
                    else
                    {
                        ModelState.AddModelError("", "Incorrect username or password");
                    }
                }
                else
                    ModelState.AddModelError("", "Incorrect form data");
            }
            return View(loginModel);
        }
        [Authorize]
        public ActionResult Logout()
        {
            var logout = HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);

            return RedirectToAction("Index", "Home");
        }
        private async Task Authenticate(User user)
        {
            var claims = new List<Claim>
            {
                new Claim(ClaimsIdentity.DefaultNameClaimType, user.Email),
                new Claim(ClaimsIdentity.DefaultRoleClaimType, user.Role?.Name)
            };
            ClaimsIdentity id = new ClaimsIdentity(claims, "ApplicationCookie", ClaimsIdentity.DefaultNameClaimType, ClaimsIdentity.DefaultRoleClaimType);
            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(id));

        }
        static public bool HasAdminRights(ClaimsPrincipal User)
        {
            string role = ((ClaimsIdentity)User.Identity).Claims
                 .Where(c => c.Type == ClaimTypes.Role)
                 .Select(c => c.Value).FirstOrDefault();
            if (role == "admin")
                return true;
            return false;
        }
    }
}