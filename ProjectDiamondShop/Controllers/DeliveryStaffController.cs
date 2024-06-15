using DiamondShopBOs;
using DiamondShopServices;
using DiamondShopServices.StaffServices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace ProjectDiamondShop.Controllers
{
    public class DeliveryStaffController : Controller
    {
        private readonly IStaffService _staffService;
        private bool IsAdmin()
        {
            return Session["RoleID"] != null && (int)Session["RoleID"] == 2;
        }
        private bool IsSaleStaff()
        {
            return Session["RoleID"] != null && (int)Session["RoleID"] == 5;
        }
        private bool IsManager()
        {
            return Session["RoleID"] != null && (int)Session["RoleID"] == 3;
        }

        public DeliveryStaffController()
        {
            _staffService = new StaffService();
        }

        // GET: DeliveryStaff
        public ActionResult Index(string searchOrderId)
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
            if (Session["RoleID"] == null || (int)Session["RoleID"] != 4)
            {
                return RedirectToAction("Index", "Home");
            }

            string deliveryStaffID = Session["UserID"].ToString();
            List<tblOrder> orders = _staffService.GetOrdersByStaffId(deliveryStaffID, 4, searchOrderId);
            ViewBag.Orders = orders;
            return View("DeliveryStaff");
        }
    }
}
