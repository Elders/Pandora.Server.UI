using System;
using System.Linq;
using Microsoft.AspNet.Authorization;
using Microsoft.AspNet.Http;
using Microsoft.AspNet.Mvc;

namespace Elders.Pandora.Server.UI.Controllers
{
    [Authorize]
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Logout()
        {
            var cks = HttpContext.Request.Cookies.Keys.Cast<string>().ToList();
            foreach (string item in cks)
            {
                if (HttpContext.Request.Cookies.ContainsKey(item))
                {
                    HttpContext.Response.Cookies.Append(item, string.Empty, new CookieOptions() { Expires = DateTime.Now.AddDays(-1) });
                }
            }

            return Redirect("/Login");
        }

        public IActionResult Error()
        {
            return View();
        }
    }
}
