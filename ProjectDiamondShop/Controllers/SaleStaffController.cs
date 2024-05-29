using ProjectDiamondShop.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Web.Mvc;

namespace ProjectDiamondShop.Controllers
{
    public class SaleStaffController : Controller
    {
        private readonly string connectionString = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;

        // GET: SaleStaff
        public ActionResult Index()
        {
            if (Session["RoleID"] == null || (int)Session["RoleID"] != 5)
            {
                return RedirectToAction("Index", "Home");
            }

            List<Order> orders = GetOrders();
            return View(orders);
        }

        // Process Order
        [HttpPost]
        public ActionResult Process(string orderId)
        {
            UpdateOrderStatus(orderId, "Preparing");
            return RedirectToAction("Index");
        }

        // View Order Details
        public ActionResult ViewOrder(string orderId)
        {
            Order order = GetOrderDetails(orderId);
            return View(order);
        }

        // Update Order Status
        [HttpPost]
        public ActionResult UpdateStatus(string orderId, string status)
        {
            UpdateOrderStatus(orderId, status);
            return RedirectToAction("ViewOrder", new { orderId });
        }

        // Get Update Status Page
        public ActionResult UpdateOrderStatus(string orderId)
        {
            var order = GetOrderDetails(orderId);
            ViewBag.OrderId = orderId;
            return View(order);
        }

        private List<Order> GetOrders()
        {
            List<Order> orders = new List<Order>();

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand("SELECT orderID, customerID, deliveryStaffID, totalMoney, status, address, phone, saleDate FROM tblOrder", conn);
                SqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    orders.Add(new Order
                    {
                        OrderID = reader["orderID"].ToString(),
                        CustomerID = reader["customerID"].ToString(),
                        DeliveryStaffID = reader["deliveryStaffID"].ToString(),
                        TotalMoney = Convert.ToDouble(reader["totalMoney"]),
                        Status = reader["status"].ToString(),
                        Address = reader["address"].ToString(),
                        Phone = reader["phone"].ToString(),
                        SaleDate = Convert.ToDateTime(reader["saleDate"])
                    });
                }
            }

            return orders;
        }

        private Order GetOrderDetails(string orderId)
        {
            Order order = null;

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand("SELECT orderID, customerID, deliveryStaffID, totalMoney, status, address, phone, saleDate FROM tblOrder WHERE orderID = @OrderID", conn);
                cmd.Parameters.AddWithValue("@OrderID", orderId);
                SqlDataReader reader = cmd.ExecuteReader();
                if (reader.Read())
                {
                    order = new Order
                    {
                        OrderID = reader["orderID"].ToString(),
                        CustomerID = reader["customerID"].ToString(),
                        DeliveryStaffID = reader["deliveryStaffID"].ToString(),
                        TotalMoney = Convert.ToDouble(reader["totalMoney"]),
                        Status = reader["status"].ToString(),
                        Address = reader["address"].ToString(),
                        Phone = reader["phone"].ToString(),
                        SaleDate = Convert.ToDateTime(reader["saleDate"])
                    };
                }
            }

            return order;
        }

        private void UpdateOrderStatus(string orderId, string status)
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand("UPDATE tblOrder SET status = @Status WHERE orderID = @OrderID", conn);
                cmd.Parameters.AddWithValue("@Status", status);
                cmd.Parameters.AddWithValue("@OrderID", orderId);
                cmd.ExecuteNonQuery();
            }
        }
    }
}
