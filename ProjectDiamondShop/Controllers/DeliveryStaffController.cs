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

        public DeliveryStaffController()
        {
            _staffService = new StaffService();
        }

        // GET: DeliveryStaff
        public ActionResult Index(string searchOrderId)
        {
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
