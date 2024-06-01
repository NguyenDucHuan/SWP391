using ProjectDiamondShop.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;
using System.Web.Mvc;

namespace ProjectDiamondShop.Controllers
{
    public class ManagerController : Controller
    {
        private readonly string connectionString = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;

        // GET: Manager
        public ActionResult Index()
        {
            if (Session["RoleID"] == null || (int)Session["RoleID"] != 3)
            {
                return RedirectToAction("Index", "Home");
            }

            var model = new ManagerViewModel
            {
                Orders = GetOrders(),
                Diamonds = GetDiamonds(),
                SaleStaff = GetSaleStaff(),
                DeliveryStaff = GetDeliveryStaff()
            };

            var revenueData = GetRevenueData();
            ViewBag.RevenueLabels = revenueData.Select(r => r.Date).ToList();
            ViewBag.RevenueData = revenueData.Select(r => r.Revenue).ToList();

            return View(model);
        }

        [HttpPost]
        public ActionResult UpdateOrderStatus(string orderId, string newStatus)
        {
            if (Session["RoleID"] == null || (int)Session["RoleID"] != 3)
            {
                return new HttpStatusCodeResult(403);
            }

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();

                // Kiểm tra trạng thái hiện tại của đơn hàng
                SqlCommand cmdCheckStatus = new SqlCommand("SELECT status FROM tblOrder WHERE orderID = @OrderID", conn);
                cmdCheckStatus.Parameters.AddWithValue("@OrderID", orderId);
                var currentStatus = cmdCheckStatus.ExecuteScalar()?.ToString();

                // Đảm bảo trạng thái được cập nhật tuần tự
                if (!IsValidStatusTransition(currentStatus, newStatus))
                {
                    return new HttpStatusCodeResult(400, "Invalid status transition");
                }

                // Cập nhật trạng thái mới và lưu thời gian cập nhật
                SqlCommand cmdUpdateStatus = new SqlCommand("UPDATE tblOrder SET status = @NewStatus WHERE orderID = @OrderID", conn);
                cmdUpdateStatus.Parameters.AddWithValue("@NewStatus", newStatus);
                cmdUpdateStatus.Parameters.AddWithValue("@OrderID", orderId);
                cmdUpdateStatus.ExecuteNonQuery();

                // Lưu thời gian cập nhật trạng thái
                SqlCommand cmdLogStatusUpdate = new SqlCommand("INSERT INTO tblOrderStatusUpdates (orderID, status, updateTime) VALUES (@OrderID, @Status, @UpdateTime)", conn);
                cmdLogStatusUpdate.Parameters.AddWithValue("@OrderID", orderId);
                cmdLogStatusUpdate.Parameters.AddWithValue("@Status", newStatus);
                cmdLogStatusUpdate.Parameters.AddWithValue("@UpdateTime", DateTime.Now);
                cmdLogStatusUpdate.ExecuteNonQuery();
            }

            return new HttpStatusCodeResult(200);
        }

        private bool IsValidStatusTransition(string currentStatus, string newStatus)
        {
            var statusOrder = new List<string>
            {
                "Order Placed",
                "Preparing Goods",
                "Shipped to Carrier",
                "In Delivery",
                "Delivered",
                "Paid"
            };

            var currentIndex = statusOrder.IndexOf(currentStatus);
            var newIndex = statusOrder.IndexOf(newStatus);

            // Trạng thái mới phải lớn hơn trạng thái hiện tại
            return newIndex > currentIndex;
        }

        private List<Order> GetOrders()
        {
            List<Order> orders = new List<Order>();

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand("SELECT orderID, customerID, saleStaffID, deliveryStaffID, totalMoney, status, address, phone, saleDate FROM tblOrder", conn);
                SqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    orders.Add(new Order
                    {
                        OrderID = reader["orderID"].ToString(),
                        CustomerID = reader["customerID"].ToString(),
                        SaleStaffID = reader["saleStaffID"].ToString(),
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

        private List<Diamond> GetDiamonds()
        {
            List<Diamond> diamonds = new List<Diamond>();

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand("SELECT * FROM tblDiamonds", conn);
                SqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    diamonds.Add(new Diamond
                    {
                        diamondID = Convert.ToInt32(reader["diamondID"]),
                        diamondName = reader["diamondName"].ToString(),
                        diamondPrice = Convert.ToDecimal(reader["diamondPrice"]),
                        diamondDescription = reader["diamondDescription"].ToString(),
                        caratWeight = Convert.ToSingle(reader["caratWeight"]),
                        clarityID = reader["clarityID"].ToString(),
                        cutID = reader["cutID"].ToString(),
                        colorID = reader["colorID"].ToString(),
                        shapeID = reader["shapeID"].ToString(),
                        diamondImagePath = reader["diamondImagePath"].ToString(),
                        status = Convert.ToBoolean(reader["status"])
                    });
                }
            }

            return diamonds;
        }

        private List<User> GetSaleStaff()
        {
            return GetUsersByRole(5);
        }

        private List<User> GetDeliveryStaff()
        {
            return GetUsersByRole(4);
        }

        private List<User> GetUsersByRole(int roleID)
        {
            List<User> users = new List<User>();

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand("SELECT userID, userName, fullName, email, status FROM tblUsers WHERE roleID = @RoleID", conn);
                cmd.Parameters.AddWithValue("@RoleID", roleID);
                SqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    users.Add(new User
                    {
                        UserID = reader["userID"].ToString(),
                        UserName = reader["userName"].ToString(),
                        FullName = reader["fullName"].ToString(),
                        Email = reader["email"].ToString(),
                        Status = Convert.ToBoolean(reader["status"])
                    });
                }
            }

            return users;
        }

        private List<RevenueData> GetRevenueData()
        {
            List<RevenueData> revenueData = new List<RevenueData>();

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand("SELECT CONVERT(VARCHAR, saleDate, 23) as Date, SUM(totalMoney) as Revenue FROM tblOrder GROUP BY CONVERT(VARCHAR, saleDate, 23) ORDER BY Date", conn);
                SqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    revenueData.Add(new RevenueData
                    {
                        Date = reader["Date"].ToString(),
                        Revenue = Convert.ToDouble(reader["Revenue"])
                    });
                }
            }

            return revenueData;
        }
    }

    public class RevenueData
    {
        public string Date { get; set; }
        public double Revenue { get; set; }
    }
}
