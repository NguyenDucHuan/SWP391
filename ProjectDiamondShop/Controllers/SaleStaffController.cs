using ProjectDiamondShop.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;
using System.Net;
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

            var saleStaffID = Session["UserID"].ToString();
            List<Order> orders = GetOrders(saleStaffID);
            return View("SaleStaff", orders);
        }

        [HttpPost]
        public ActionResult Process(string orderId)
        {
            if (string.IsNullOrEmpty(orderId))
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest, "Order ID is required");
            }

            var currentStatus = GetCurrentStatus(orderId);
            if (currentStatus != "Order Placed")
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest, "Invalid status transition");
            }

            UpdateOrderStatus(orderId, "Prepare goods");
            return RedirectToAction("Index");
        }

        public ActionResult UpdateOrderDetails(string orderId)
        {
            if (string.IsNullOrEmpty(orderId))
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest, "Order ID is required");
            }

            var order = GetOrderDetails(orderId);
            ViewBag.StatusUpdates = GetStatusUpdates(orderId); // Lấy danh sách cập nhật trạng thái
            return View(order);
        }

        private Order GetOrderDetails(string orderId)
        {
            Order order = null;

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand("SELECT orderID, customerID, deliveryStaffID, saleStaffID, totalMoney, status, address, phone, saleDate FROM tblOrder WHERE orderID = @OrderID", conn);
                cmd.Parameters.AddWithValue("@OrderID", orderId);
                SqlDataReader reader = cmd.ExecuteReader();
                if (reader.Read())
                {
                    order = new Order
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

                // Insert into tblOrderStatusUpdates
                SqlCommand cmdInsert = new SqlCommand("INSERT INTO tblOrderStatusUpdates (orderID, status, updateTime) VALUES (@OrderID, @Status, @UpdateTime)", conn);
                cmdInsert.Parameters.AddWithValue("@OrderID", orderId);
                cmdInsert.Parameters.AddWithValue("@Status", status);
                cmdInsert.Parameters.AddWithValue("@UpdateTime", DateTime.Now);
                cmdInsert.ExecuteNonQuery();
            }
        }

        private List<KeyValuePair<string, DateTime>> GetStatusUpdates(string orderId)
        {
            List<KeyValuePair<string, DateTime>> updates = new List<KeyValuePair<string, DateTime>>();

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand("SELECT status, updateTime FROM tblOrderStatusUpdates WHERE orderID = @OrderID ORDER BY updateTime", conn);
                cmd.Parameters.AddWithValue("@OrderID", orderId);
                SqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    updates.Add(new KeyValuePair<string, DateTime>(reader["status"].ToString(), Convert.ToDateTime(reader["updateTime"])));
                }
            }

            return updates;
        }

        private string GetCurrentStatus(string orderId)
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand("SELECT status FROM tblOrder WHERE orderID = @OrderID", conn);
                cmd.Parameters.AddWithValue("@OrderID", orderId);
                return cmd.ExecuteScalar()?.ToString();
            }
        }

        private List<Order> GetOrders(string saleStaffID)
        {
            List<Order> orders = new List<Order>();

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand("SELECT orderID, customerID, deliveryStaffID, saleStaffID, totalMoney, status, address, phone, saleDate FROM tblOrder WHERE saleStaffID = @SaleStaffID", conn);
                cmd.Parameters.AddWithValue("@SaleStaffID", saleStaffID);
                SqlDataReader reader = cmd.ExecuteReader();
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

            return orders;
        }
    }
}
