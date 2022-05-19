using System.Web.Mvc;
using BIZ.ExternalIntegration.ASP.MVC;
using BIZ.ExternalIntegration.Common;
using ProjectWebApi.Models;

namespace ProjectWebApi.Controllers
{
    public class HomeController : Controller
    {
        [AllowAnonymous]
        public ActionResult Index()
        {
            return View();
        }

        public new ActionResult Profile()
        {
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        public ActionResult Index(User user)
        {
            var authInfo = BIZAuthInfo.Create(user.Login, user.Password, null);
            if (BIZApplicationInitializer.RegisterSession(authInfo))
            {
                return RedirectToAction("Profile");
            }

            BIZApplicationInitializer.RemoveLocalSession();
            string message = "LogIn or password incorrect.";
            ViewBag.Message = message;
            return View();
        }

        [HttpPost]
        public ActionResult Logout()
        {
            BIZApplicationInitializer.RemoveLocalSession();
            return RedirectToAction("Index");
        }
    }
}