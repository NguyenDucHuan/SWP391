using ProjectDiamondShop.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;
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
            return View("SaleStaff", orders); // Sử dụng View SaleStaff.cshtml
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
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Process(string orderId)
        {
            if (string.IsNullOrEmpty(orderId))
            {
                TempData["UpdateMessage"] = "Order ID is required.";
                return RedirectToAction("Index");
            }

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand("UPDATE tblOrder SET status = @Status WHERE orderID = @OrderID", conn);
                cmd.Parameters.AddWithValue("@Status", "Preparing Goods");
                cmd.Parameters.AddWithValue("@OrderID", orderId);
                cmd.ExecuteNonQuery();

                // Log trạng thái cập nhật
                SqlCommand logCmd = new SqlCommand("INSERT INTO tblOrderStatusUpdates (orderID, status, updateTime) VALUES (@OrderID, @Status, @UpdateTime)", conn);
                logCmd.Parameters.AddWithValue("@OrderID", orderId);
                logCmd.Parameters.AddWithValue("@Status", "Preparing Goods");
                logCmd.Parameters.AddWithValue("@UpdateTime", DateTime.Now);
                logCmd.ExecuteNonQuery();
            }

            TempData["UpdateMessage"] = "Order updated successfully.";
            return RedirectToAction("Index");
        }

    }
}
