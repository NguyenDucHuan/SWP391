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
        public ActionResult Index(string searchOrderId)
        {
            if (Session["RoleID"] == null || (int)Session["RoleID"] != 5)
            {
                return RedirectToAction("Index", "Home");
            }

            List<Order> orders = GetOrders(searchOrderId);
            return View("SaleStaff", orders);
        }


        private string GetCurrentOrderStatus(string orderId)
        {
            using (var conn = new SqlConnection(connectionString))
            {
                conn.Open();
                var cmd = new SqlCommand("SELECT status FROM tblOrder WHERE orderID = @OrderID", conn);
                cmd.Parameters.AddWithValue("@OrderID", orderId);
                return cmd.ExecuteScalar()?.ToString();
            }
        }

        private void UpdateOrderStatus(string orderId, string status)
        {
            using (var conn = new SqlConnection(connectionString))
            {
                conn.Open();
                var cmd = new SqlCommand("UPDATE tblOrder SET status = @Status WHERE orderID = @OrderID", conn);
                cmd.Parameters.AddWithValue("@Status", status);
                cmd.Parameters.AddWithValue("@OrderID", orderId);
                cmd.ExecuteNonQuery();

                var cmdInsert = new SqlCommand("INSERT INTO tblOrderStatusUpdates (orderID, status, updateTime) VALUES (@OrderID, @Status, @UpdateTime)", conn);
                cmdInsert.Parameters.AddWithValue("@OrderID", orderId);
                cmdInsert.Parameters.AddWithValue("@Status", status);
                cmdInsert.Parameters.AddWithValue("@UpdateTime", DateTime.Now);
                cmdInsert.ExecuteNonQuery();
            }
        }

        private List<Order> GetOrders(string searchOrderId)
        {
            List<Order> orders = new List<Order>();

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand("SELECT orderID, customerID, deliveryStaffID, totalMoney, status, address, phone, saleDate FROM tblOrder WHERE saleStaffID = @SaleStaffID" +
                    (string.IsNullOrEmpty(searchOrderId) ? "" : " AND orderID LIKE @SearchOrderId"), conn);

                cmd.Parameters.AddWithValue("@SaleStaffID", Session["UserID"].ToString());
                if (!string.IsNullOrEmpty(searchOrderId))
                {
                    cmd.Parameters.AddWithValue("@SearchOrderId", "%" + searchOrderId + "%");
                }

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
    }
}
