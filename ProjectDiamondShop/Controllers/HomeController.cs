using ProjectDiamondShop.Models;
using System.Collections.Generic;
using System.Web.Mvc;
using DiamondShopServices.OrderServices;
using System.Linq;
using System;
using DiamondShopBOs;

namespace ProjectDiamondShop.Controllers
{
    public class HomeController : Controller
    {
        private readonly IOrderServices orderServices;

        public HomeController()
        {
            orderServices = new OrderServices();
        }
        private bool IsAdmin()
        {
            return Session["RoleID"] != null && (int)Session["RoleID"] == 2;
        }
        private bool IsSaleStaff()
        {
            return Session["RoleID"] != null && (int)Session["RoleID"] == 5;
        }
        private bool IsDelivery()
        {
            return Session["RoleID"] != null && (int)Session["RoleID"] == 4;
        }
        private bool IsManager()
        {
            return Session["RoleID"] != null && (int)Session["RoleID"] == 3;
        }
        public ActionResult Index()
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
            if (TempData["SuccessMessage"] != null)
            {
                ViewBag.SuccessMessage = TempData["SuccessMessage"].ToString();
                ViewBag.UserName = TempData["UserName"].ToString();
            }
            return View("HomePage");
        }

        public ActionResult Contact()
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

        public ActionResult About()
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

        public ActionResult FAQ()
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

        public ActionResult ViewOrders(int page = 1, int pageSize = 10)
        {
            if (IsAdmin() || IsManager())
            {
                return RedirectToAction("Index", "Manager");
            }

            var userID = Session["UserID"]?.ToString();
            if (string.IsNullOrEmpty(userID))
            {
                return RedirectToAction("Index", "Account");
            }

            var allCurrentOrders = orderServices.GetOrdersByStatus(userID, new[] { "Order Placed", "Preparing Goods", "Shipped to Carrier", "In Delivery" });
            var allHistoryOrders = orderServices.GetOrdersByStatus(userID, new[] { "Delivered", "Paid" }, true);

            var currentOrders = allCurrentOrders.Skip((page - 1) * pageSize).Take(pageSize).ToList();
            var historyOrders = allHistoryOrders.Skip((page - 1) * pageSize).Take(pageSize).ToList();

            ViewBag.TotalCurrentOrders = allCurrentOrders.Count();
            ViewBag.TotalHistoryOrders = allHistoryOrders.Count();
            ViewBag.CurrentPage = page;
            ViewBag.PageSize = pageSize;

            var warranties = new Dictionary<string, List<string>>();
            foreach (var order in currentOrders.Concat(historyOrders))
            {
                var orderWarranties = orderServices.GetWarrantiesByOrderID(order.orderID)
                    .Select(w => w.warrantyCode).ToList();
                warranties[order.orderID] = orderWarranties;
            }
            ViewBag.Warranties = warranties;

            OrderViewModel orderViewModel = new OrderViewModel(currentOrders, historyOrders);

            // Giả sử bạn lấy tên nhân viên giao hàng từ cơ sở dữ liệu hoặc từ danh sách đơn hàng
            if (currentOrders.Any())
            {
                ViewBag.deliveryStaffName = currentOrders.FirstOrDefault()?.deliveryStaffName;
            }

            return View("ViewOrder", orderViewModel);
        }

    }
}
