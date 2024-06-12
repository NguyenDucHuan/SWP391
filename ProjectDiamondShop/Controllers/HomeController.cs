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

        public ActionResult ViewOrders()
        {

            var userID = Session["UserID"]?.ToString();
            if (string.IsNullOrEmpty(userID))
            {
                return RedirectToAction("Index", "Login");
            }
            var currentOrders = orderServices.GetOrdersByStatus(userID, new[] { "Order Placed", "Preparing Goods", "Shipped to Carrier", "In Delivery" });
            var historyOrders = orderServices.GetOrdersByStatus(userID, new[] { "Delivered", "Paid" }, true);
            OrderViewModel orderViewModel = new OrderViewModel(currentOrders, historyOrders);
            return View("ViewOrder", orderViewModel);
        }
    }
}
