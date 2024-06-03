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
            if (string.IsNullOrEmpty(userID))
            {
                return RedirectToAction("Index", "Login");
            }

            var currentOrders = GetOrdersByStatus(userID, new[] { "Order Placed", "Preparing Goods", "Shipped to Carrier", "In Delivery" });
            var historyOrders = GetOrdersByStatus(userID, new[] { "Delivered", "Paid" }, true);

            var model = new ViewOrderViewModel
            {
                CurrentOrders = currentOrders,
                HistoryOrders = historyOrders,
                RoleID = Convert.ToInt32(Session["RoleID"])
            };

            return View("ViewOrder", model);
        }


        private List<Order> GetOrdersByStatus(string userID, string[] statuses, bool isHistory = false)
        {
            var orders = new List<Order>();

            using (var conn = new SqlConnection(connectionString))
            {
                conn.Open();
                var query = @"
        SELECT orderID, customerID, deliveryStaffID, saleStaffID, totalMoney, status, address, phone, saleDate 
        FROM tblOrder 
        WHERE customerID = @CustomerID 
        AND status IN (" + string.Join(",", statuses.Select(s => $"'{s}'")) + @")";

                if (!isHistory)
                {
                    query += " OR (deliveryStaffID IS NULL OR saleStaffID IS NULL)";
                }

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
                                DeliveryStaffID = reader["deliveryStaffID"] != DBNull.Value ? reader["deliveryStaffID"].ToString() : null,
                                SaleStaffID = reader["saleStaffID"] != DBNull.Value ? reader["saleStaffID"].ToString() : null,
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
