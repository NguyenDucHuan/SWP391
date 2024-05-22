using System.Web.Mvc;

namespace ProjectDiamondShop.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            if (TempData["SuccessMessage"] != null)
            {
                ViewBag.SuccessMessage = TempData["SuccessMessage"].ToString();
                ViewBag.UserName = TempData["UserName"].ToString();
            }
            return View("HomePage");
        }

        public ActionResult Contact()
        {
            return View();
        }

        public ActionResult About()
        {
            return View();
        }
    }
}
