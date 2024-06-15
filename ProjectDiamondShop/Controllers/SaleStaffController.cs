using DiamondShopBOs;
using DiamondShopServices;
using DiamondShopServices.StaffServices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace ProjectDiamondShop.Controllers
{
    public class SaleStaffController : Controller
    {
        private readonly IStaffService _staffService;
        private bool IsAdmin()
        {
            return Session["RoleID"] != null && (int)Session["RoleID"] == 2;
        }
        private bool IsDelivery()
        {
            return Session["RoleID"] != null && (int)Session["RoleID"] == 4;
        }
        private bool IsManager()
        {
            return Session["RoleID"] != null && (int)Session["RoleID"] == 3;
        }

        public SaleStaffController()
        {
            _staffService = new StaffService();
        }

        // GET: SaleStaff
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
            if (IsDelivery())
            {
                return RedirectToAction("Index", "DeliveryStaff");
            }
            if (Session["RoleID"] == null || (int)Session["RoleID"] != 5)
            {
                return RedirectToAction("Index", "Home");
            }

            string saleStaffID = Session["UserID"].ToString();
            List<tblOrder> orders = _staffService.GetOrdersByStaffId(saleStaffID, 5, searchOrderId);
            ViewBag.Orders = orders;
            return View("SaleStaff");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Process(string orderId)
        {
            if (IsAdmin())
            {
                return RedirectToAction("Index", "Manager");
            }
            if (IsManager())
            {
                return RedirectToAction("Index", "Manager");
            }
            if (IsDelivery())
            {
                return RedirectToAction("Index", "DeliveryStaff");
            }
            if (string.IsNullOrEmpty(orderId))
            {
                TempData["UpdateMessage"] = "Order ID is required.";
                return RedirectToAction("Index");
            }

            var order = _staffService.GetOrderById(orderId);
            if (order != null)
            {
                _staffService.UpdateOrderStatus(orderId, "Preparing Goods");
                TempData["UpdateMessage"] = "Order updated successfully.";
            }
            else
            {
                TempData["UpdateMessage"] = "Order not found.";
            }

            return RedirectToAction("Index");
        }
    }
}
