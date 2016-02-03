using Microsoft.AspNet.Authorization;
using Microsoft.AspNet.Mvc;

namespace Elders.Pandora.Server.UI.Controllers
{
    public class LoginController : Controller
    {
        [AllowAnonymous]
        public ActionResult Index()
        {
            return View();
        }

        [Authorize]
        public ActionResult Oauth()
        {
            return Redirect("/Home");
        }
    }
}
