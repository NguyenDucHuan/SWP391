using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ProjectDiamondShop.Controllers
{
    public class IntroduceController : Controller
    {
        // GET: Introduce
        public ActionResult Round()
        {
            if (IsAdmin())
            {
                return RedirectToAction("Index", "Manager");
            }
            if (IsManager())
            {
                return RedirectToAction("Index", "Manager");
            }
            if (IsSaleStaff())
            {
                return RedirectToAction("Index", "SaleStaff");
            }
            if (IsDelivery())
            {
                return RedirectToAction("Index", "DeliveryStaff");
            }
            return View();
        }

        public ActionResult Heart()
        {
            if (IsAdmin())
            {
                return RedirectToAction("Index", "Manager");
            }
            if (IsManager())
            {
                return RedirectToAction("Index", "Manager");
            }
            if (IsSaleStaff())
            {
                return RedirectToAction("Index", "SaleStaff");
            }
            if (IsDelivery())
            {
                return RedirectToAction("Index", "DeliveryStaff");
            }
            return View();
        }

        public ActionResult Oval()
        {
            if (IsAdmin())
            {
                return RedirectToAction("Index", "Manager");
            }
            if (IsManager())
            {
                return RedirectToAction("Index", "Manager");
            }
            if (IsSaleStaff())
            {
                return RedirectToAction("Index", "SaleStaff");
            }
            if (IsDelivery())
            {
                return RedirectToAction("Index", "DeliveryStaff");
            }
            return View();
        }

        public ActionResult Pear()
        {
            if (IsAdmin())
            {
                return RedirectToAction("Index", "Manager");
            }
            if (IsManager())
            {
                return RedirectToAction("Index", "Manager");
            }
            if (IsSaleStaff())
            {
                return RedirectToAction("Index", "SaleStaff");
            }
            if (IsDelivery())
            {
                return RedirectToAction("Index", "DeliveryStaff");
            }
            return View();
        }

        public ActionResult Cushion()
        {
            if (IsAdmin())
            {
                return RedirectToAction("Index", "Manager");
            }
            if (IsManager())
            {
                return RedirectToAction("Index", "Manager");
            }
            if (IsSaleStaff())
            {
                return RedirectToAction("Index", "SaleStaff");
            }
            if (IsDelivery())
            {
                return RedirectToAction("Index", "DeliveryStaff");
            }
            return View();
        }

        public ActionResult Radiant()
        {
            if (IsAdmin())
            {
                return RedirectToAction("Index", "Manager");
            }
            if (IsManager())
            {
                return RedirectToAction("Index", "Manager");
            }
            if (IsSaleStaff())
            {
                return RedirectToAction("Index", "SaleStaff");
            }
            if (IsDelivery())
            {
                return RedirectToAction("Index", "DeliveryStaff");
            }
            return View();
        }

        public ActionResult Asscher()
        {
            if (IsAdmin())
            {
                return RedirectToAction("Index", "Manager");
            }
            if (IsManager())
            {
                return RedirectToAction("Index", "Manager");
            }
            if (IsSaleStaff())
            {
                return RedirectToAction("Index", "SaleStaff");
            }
            if (IsDelivery())
            {
                return RedirectToAction("Index", "DeliveryStaff");
            }
            return View();
        }

        public ActionResult Baguette()
        {
            if (IsAdmin())
            {
                return RedirectToAction("Index", "Manager");
            }
            if (IsManager())
            {
                return RedirectToAction("Index", "Manager");
            }
            if (IsSaleStaff())
            {
                return RedirectToAction("Index", "SaleStaff");
            }
            if (IsDelivery())
            {
                return RedirectToAction("Index", "DeliveryStaff");
            }
            return View();
        }

        public ActionResult Emerald()
        {
            if (IsAdmin())
            {
                return RedirectToAction("Index", "Manager");
            }
            if (IsManager())
            {
                return RedirectToAction("Index", "Manager");
            }
            if (IsSaleStaff())
            {
                return RedirectToAction("Index", "SaleStaff");
            }
            if (IsDelivery())
            {
                return RedirectToAction("Index", "DeliveryStaff");
            }
            return View();
        }

        public ActionResult Marquise()
        {
            if (IsAdmin())
            {
                return RedirectToAction("Index", "Manager");
            }
            if (IsManager())
            {
                return RedirectToAction("Index", "Manager");
            }
            if (IsSaleStaff())
            {
                return RedirectToAction("Index", "SaleStaff");
            }
            if (IsDelivery())
            {
                return RedirectToAction("Index", "DeliveryStaff");
            }
            return View();
        }

        public ActionResult Princess()
        {
            if (IsAdmin())
            {
                return RedirectToAction("Index", "Manager");
            }
            if (IsManager())
            {
                return RedirectToAction("Index", "Manager");
            }
            if (IsSaleStaff())
            {
                return RedirectToAction("Index", "SaleStaff");
            }
            if (IsDelivery())
            {
                return RedirectToAction("Index", "DeliveryStaff");
            }
            return View();
        }

        public ActionResult Trillion()
        {
            if (IsAdmin())
            {
                return RedirectToAction("Index", "Manager");
            }
            if (IsManager())
            {
                return RedirectToAction("Index", "Manager");
            }
            if (IsSaleStaff())
            {
                return RedirectToAction("Index", "SaleStaff");
            }
            if (IsDelivery())
            {
                return RedirectToAction("Index", "DeliveryStaff");
            }
            return View();
        }

        private bool IsAdmin()
        {
            return Session["RoleID"] != null && (int)Session["RoleID"] == 2;
        }

        private bool IsSaleStaff()
        {
            return Session["RoleID"] != null && (int)Session["RoleID"] == 4;
        }

        private bool IsDelivery()
        {
            return Session["RoleID"] != null && (int)Session["RoleID"] == 5;
        }

        private bool IsManager()
        {
            return Session["RoleID"] != null && (int)Session["RoleID"] == 3;
        }
    }
}
