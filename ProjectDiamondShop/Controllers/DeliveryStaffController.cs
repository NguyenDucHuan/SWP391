using ProjectDiamondShop.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Web.Mvc;

namespace ProjectDiamondShop.Controllers
{
    public class DeliveryStaffController : Controller
    {
        private readonly string connectionString = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;

        // GET: DeliveryStaff
        public ActionResult Index()
        {
            if (Session["RoleID"] == null || (int)Session["RoleID"] != 4)
            {
                return RedirectToAction("Index", "Home");
            }

            List<Order> orders = GetOrders();
            return View("DeliveryStaff", orders); // Sử dụng View DeliveryStaff.cshtml
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
    }
}
