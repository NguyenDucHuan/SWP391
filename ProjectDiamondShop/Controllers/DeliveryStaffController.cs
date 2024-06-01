using ProjectDiamondShop.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;
using System.Web.Mvc;

namespace ProjectDiamondShop.Controllers
{
    public class DeliveryStaffController : Controller
    {
        private readonly string connectionString = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;

        // GET: DeliveryStaff
        public ActionResult Index(string searchOrderId)
        {
            if (Session["RoleID"] == null || (int)Session["RoleID"] != 4)
            {
                return RedirectToAction("Index", "Home");
            }

            List<Order> orders = GetOrders(searchOrderId);
            return View("DeliveryStaff", orders); // Sử dụng View DeliveryStaff.cshtml
        }

        private List<Order> GetOrders(string searchOrderId)
        {
            List<Order> orders = new List<Order>();

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand("SELECT orderID, customerID, deliveryStaffID, totalMoney, status, address, phone, saleDate FROM tblOrder WHERE deliveryStaffID = @DeliveryStaffID" +
                    (string.IsNullOrEmpty(searchOrderId) ? "" : " AND orderID LIKE @SearchOrderId"), conn);

                cmd.Parameters.AddWithValue("@DeliveryStaffID", Session["UserID"].ToString());
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
