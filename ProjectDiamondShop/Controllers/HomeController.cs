using ProjectDiamondShop.Models;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System;
using System.Web.Mvc;
using System.Configuration;

namespace ProjectDiamondShop.Controllers
{
    public class HomeController : Controller
    {
        private readonly string connectionString = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;
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
            var roleID = Session["RoleID"]?.ToString();
            if (string.IsNullOrEmpty(userID))
            {
                return RedirectToAction("Index", "Login");
            }

            var currentOrders = GetOrdersByStatus(userID, new[] { "Order Placed", "Paid", "Shipped to Carrier", "In Delivery" });
            var historyOrders = GetOrdersByStatus(userID, new[] { "Delivered", "Returned" });

            ViewBag.CurrentOrders = currentOrders;
            ViewBag.HistoryOrders = historyOrders;
            ViewBag.RoleID = roleID;

            return View("ViewOrder");
        }

        private List<Order> GetOrdersByStatus(string userID, string[] statuses)
        {
            var orders = new List<Order>();

            using (var conn = new SqlConnection(connectionString))
            {
                conn.Open();
                var query = "SELECT orderID, customerID, deliveryStaffID, saleStaffID, totalMoney, status, address, phone, saleDate FROM tblOrder WHERE customerID = @CustomerID AND status IN (" + string.Join(",", statuses.Select(s => $"'{s}'")) + ")";
                using (var cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@CustomerID", userID);

                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            orders.Add(new Order
                            {
                                OrderID = reader["orderID"].ToString(),
                                CustomerID = reader["customerID"].ToString(),
                                DeliveryStaffID = reader["deliveryStaffID"].ToString(),
                                SaleStaffID = reader["saleStaffID"].ToString(),
                                TotalMoney = Convert.ToDouble(reader["totalMoney"]),
                                Status = reader["status"].ToString(),
                                Address = reader["address"].ToString(),
                                Phone = reader["phone"].ToString(),
                                SaleDate = Convert.ToDateTime(reader["saleDate"])
                            });
                        }
                    }
                }
            }

            return orders;
        }
    }
}
