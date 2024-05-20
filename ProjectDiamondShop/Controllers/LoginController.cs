using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ProjectDiamondShop.Controllers
{
    public class LoginController : Controller
    {
        //GET LOGIN
        public ActionResult Index()
        {
            return View("LoginPage");
        }
        public ActionResult LoginPage()
        {
            return View();
        }
    }
}