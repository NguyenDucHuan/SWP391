using ProjectDiamondShop.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Text.RegularExpressions;

namespace ProjectDiamondShop.Controllers
{
    public class ManagerController : Controller
    {
        private readonly string connectionString = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;

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

        public ActionResult CreateVoucher()
        {
            if (Session["RoleID"] == null || (int)Session["RoleID"] != 3)
            {
                return RedirectToAction("Index", "Home");
            }

            return View(new Voucher());
        }

        [HttpPost]
        public ActionResult CreateVoucher(Voucher voucher, string assignTo)
        {
            if (Session["RoleID"] == null || (int)Session["RoleID"] != 3)
            {
                return RedirectToAction("Index", "Home");
            }

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();
                string voucherID = GenerateUniqueVoucherID(conn);

                SqlCommand cmd = new SqlCommand("INSERT INTO tblVoucher (voucherID, startTime, endTime, discount, quantity, status) VALUES (@VoucherID, @StartTime, @EndTime, @Discount, @Quantity, 1);", conn);
                cmd.Parameters.AddWithValue("@VoucherID", voucherID);
                cmd.Parameters.AddWithValue("@StartTime", voucher.StartTime);
                cmd.Parameters.AddWithValue("@EndTime", voucher.EndTime);
                cmd.Parameters.AddWithValue("@Discount", voucher.Discount);
                cmd.Parameters.AddWithValue("@Quantity", voucher.Quantity);
                cmd.ExecuteNonQuery();

                if (assignTo == "all")
                {
                    SqlCommand cmdAll = new SqlCommand("INSERT INTO tblVoucherCatch (voucherID, userID) SELECT @VoucherID, userID FROM tblUsers", conn);
                    cmdAll.Parameters.AddWithValue("@VoucherID", voucherID);
                    cmdAll.ExecuteNonQuery();
                }
                else
                {
                    SqlCommand cmdUser = new SqlCommand("INSERT INTO tblVoucherCatch (voucherID, userID) VALUES (@VoucherID, @UserID)", conn);
                    cmdUser.Parameters.AddWithValue("@VoucherID", voucherID);
                    cmdUser.Parameters.AddWithValue("@UserID", assignTo);
                    cmdUser.ExecuteNonQuery();
                }
            }

            return RedirectToAction("Index");
        }

        private string GenerateUniqueVoucherID(SqlConnection conn)
        {
            string voucherID;
            Random random = new Random();
            do
            {
                voucherID = new string(Enumerable.Repeat("ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789", 6)
                    .Select(s => s[random.Next(s.Length)]).ToArray());
            } while (VoucherIDExists(conn, voucherID));
            return voucherID;
        }

        private bool VoucherIDExists(SqlConnection conn, string voucherID)
        {
            SqlCommand cmd = new SqlCommand("SELECT COUNT(*) FROM tblVoucher WHERE voucherID = @VoucherID", conn);
            cmd.Parameters.AddWithValue("@VoucherID", voucherID);
            int count = (int)cmd.ExecuteScalar();
            return count > 0;
        }

        [HttpPost]
        public ActionResult AssignStaff(string orderId, string staffId, string staffType)
        {
            if (Session["RoleID"] == null || (int)Session["RoleID"] != 3)
            {
                return new HttpStatusCodeResult(403);
            }

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();
                string column = staffType == "sale" ? "saleStaffID" : "deliveryStaffID";
                SqlCommand cmd = new SqlCommand($"UPDATE tblOrder SET {column} = @StaffID WHERE orderID = @OrderID", conn);
                cmd.Parameters.AddWithValue("@OrderID", orderId);
                cmd.Parameters.AddWithValue("@StaffID", staffId);
                cmd.ExecuteNonQuery();
            }

            return new HttpStatusCodeResult(200);
        }

        public ActionResult AddDiamond()
        {
            if (Session["RoleID"] == null || (int)Session["RoleID"] != 3)
            {
                return RedirectToAction("Index", "Home");
            }
            return View();
        }

        [HttpPost]
        public ActionResult AddDiamond(Diamond model, HttpPostedFileBase diamondImageA, HttpPostedFileBase diamondImageB, HttpPostedFileBase diamondImageC, HttpPostedFileBase certificateImage)
        {
            if (Session["RoleID"] == null || (int)Session["RoleID"] != 3)
            {
                return RedirectToAction("Index", "Home");
            }

            if (ModelState.IsValid)
            {
                string diamondImagePaths = "";
                string certificateImagePath = "";

                // Get the next available diamond image number
                int nextImageNumber = GetNextDiamondImageNumber();

                // Save diamond images
                if (diamondImageA != null && diamondImageA.ContentLength > 0)
                {
                    string fileName = $"dia{nextImageNumber}A.png";
                    string path = Path.Combine(Server.MapPath("~/Image/DiamondDTO/Diamonds"), fileName);
                    diamondImageA.SaveAs(path);
                    diamondImagePaths += $"/Image/DiamondDTO/Diamonds/{fileName}|";
                }

                if (diamondImageB != null && diamondImageB.ContentLength > 0)
                {
                    string fileName = $"dia{nextImageNumber}B.png";
                    string path = Path.Combine(Server.MapPath("~/Image/DiamondDTO/Diamonds"), fileName);
                    diamondImageB.SaveAs(path);
                    diamondImagePaths += $"/Image/DiamondDTO/Diamonds/{fileName}|";
                }

                if (diamondImageC != null && diamondImageC.ContentLength > 0)
                {
                    string fileName = $"dia{nextImageNumber}C.png";
                    string path = Path.Combine(Server.MapPath("~/Image/DiamondDTO/Diamonds"), fileName);
                    diamondImageC.SaveAs(path);
                    diamondImagePaths += $"/Image/DiamondDTO/Diamonds/{fileName}|";
                }

                diamondImagePaths = diamondImagePaths.TrimEnd('|');

                // Save certificate image
                if (certificateImage != null && certificateImage.ContentLength > 0)
                {
                    string fileName = $"CER{nextImageNumber:D2}.jpg";
                    string path = Path.Combine(Server.MapPath("~/Image/DiamondDTO/Certificates"), fileName);
                    certificateImage.SaveAs(path);
                    certificateImagePath = $"/Image/DiamondDTO/Certificates/{fileName}";
                }

                // Insert diamond into database
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();
                    SqlCommand cmd = new SqlCommand("INSERT INTO tblDiamonds (diamondName, diamondPrice, diamondDescription, caratWeight, clarityID, cutID, colorID, shapeID, diamondImagePath, status) OUTPUT INSERTED.diamondID VALUES (@diamondName, @diamondPrice, @diamondDescription, @caratWeight, @clarityID, @cutID, @colorID, @shapeID, @diamondImagePath, 1);", conn);
                    cmd.Parameters.AddWithValue("@diamondName", model.diamondName);
                    cmd.Parameters.AddWithValue("@diamondPrice", model.diamondPrice);
                    cmd.Parameters.AddWithValue("@diamondDescription", model.diamondDescription);
                    cmd.Parameters.AddWithValue("@caratWeight", model.caratWeight);
                    cmd.Parameters.AddWithValue("@clarityID", model.clarityID);
                    cmd.Parameters.AddWithValue("@cutID", model.cutID);
                    cmd.Parameters.AddWithValue("@colorID", model.colorID);
                    cmd.Parameters.AddWithValue("@shapeID", model.shapeID);
                    cmd.Parameters.AddWithValue("@diamondImagePath", diamondImagePaths);

                    int diamondID = (int)cmd.ExecuteScalar();

                    // Insert certificate into database
                    SqlCommand certCmd = new SqlCommand("INSERT INTO tblCertificate (diamondID, certificateNumber, issueDate, certifyingAuthority, cerImagePath) VALUES (@diamondID, @certificateNumber, @issueDate, @certifyingAuthority, @cerImagePath);", conn);
                    certCmd.Parameters.AddWithValue("@diamondID", diamondID);
                    certCmd.Parameters.AddWithValue("@certificateNumber", model.CertificateNumber);
                    certCmd.Parameters.AddWithValue("@issueDate", model.IssueDate);
                    certCmd.Parameters.AddWithValue("@certifyingAuthority", model.CertifyingAuthority);
                    certCmd.Parameters.AddWithValue("@cerImagePath", certificateImagePath);
                    certCmd.ExecuteNonQuery();
                }

                TempData["SuccessMessage"] = "Diamond added successfully!";
                return RedirectToAction("AddDiamond");
            }

            return View(model);
        }

        private int GetNextDiamondImageNumber()
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand("SELECT ISNULL(MAX(diamondID), 0) FROM tblDiamonds", conn);
                int maxDiamondID = (int)cmd.ExecuteScalar();
                return maxDiamondID + 1;
            }
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

        [HttpPost]
        public ActionResult UpdateOrders(List<OrderUpdateModel> orderUpdates)
        {
            if (Session["RoleID"] == null || (int)Session["RoleID"] != 3)
            {
                return Json(new { success = false, message = "Permission Denied." });
            }

            foreach (var update in orderUpdates)
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();

                    try
                    {
                        // Cập nhật SaleStaffID
                        SqlCommand cmdSaleStaff = new SqlCommand("UPDATE tblOrder SET saleStaffID = @SaleStaffID WHERE orderID = @OrderID", conn);
                        cmdSaleStaff.Parameters.AddWithValue("@SaleStaffID", update.SaleStaffID ?? (object)DBNull.Value);
                        cmdSaleStaff.Parameters.AddWithValue("@OrderID", update.OrderID);
                        cmdSaleStaff.ExecuteNonQuery();

                        // Cập nhật DeliveryStaffID
                        SqlCommand cmdDeliveryStaff = new SqlCommand("UPDATE tblOrder SET deliveryStaffID = @DeliveryStaffID WHERE orderID = @OrderID", conn);
                        cmdDeliveryStaff.Parameters.AddWithValue("@DeliveryStaffID", update.DeliveryStaffID ?? (object)DBNull.Value);
                        cmdDeliveryStaff.Parameters.AddWithValue("@OrderID", update.OrderID);
                        cmdDeliveryStaff.ExecuteNonQuery();

                        // Cập nhật Status
                        if (!string.IsNullOrEmpty(update.Status))
                        {
                            string currentStatus = GetCurrentOrderStatus(update.OrderID, conn);

                            // Nếu trạng thái mới giống như trạng thái hiện tại thì không cần kiểm tra chuyển đổi trạng thái
                            if (currentStatus != update.Status && !IsValidStatusTransition(currentStatus, update.Status))
                            {
                                return Json(new { success = false, message = $"Invalid status transition from {currentStatus} to {update.Status}. Please update again." });
                            }

                            SqlCommand cmdStatus = new SqlCommand("UPDATE tblOrder SET status = @Status WHERE orderID = @OrderID", conn);
                            cmdStatus.Parameters.AddWithValue("@Status", update.Status);
                            cmdStatus.Parameters.AddWithValue("@OrderID", update.OrderID);
                            cmdStatus.ExecuteNonQuery();

                            SqlCommand cmdLogStatusUpdate = new SqlCommand("INSERT INTO tblOrderStatusUpdates (orderID, status, updateTime) VALUES (@OrderID, @Status, @UpdateTime)", conn);
                            cmdLogStatusUpdate.Parameters.AddWithValue("@OrderID", update.OrderID);
                            cmdLogStatusUpdate.Parameters.AddWithValue("@Status", update.Status);
                            cmdLogStatusUpdate.Parameters.AddWithValue("@UpdateTime", DateTime.Now);
                            cmdLogStatusUpdate.ExecuteNonQuery();
                        }
                    }
                    catch (Exception ex)
                    {
                        return Json(new { success = false, message = $"Error updating order {update.OrderID}: {ex.Message}" });
                    }
                }
            }

            return Json(new { success = true, message = "Update Successful" });
        }

        private string GetCurrentOrderStatus(string orderId, SqlConnection conn)
        {
            SqlCommand cmd = new SqlCommand("SELECT status FROM tblOrder WHERE orderID = @OrderID", conn);
            cmd.Parameters.AddWithValue("@OrderID", orderId);
            return cmd.ExecuteScalar()?.ToString();
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

        public class RevenueData
        {
            public string Date { get; set; }
            public double Revenue { get; set; }
        }
        public class OrderUpdateModel
        {
            public string OrderID { get; set; }
            public string SaleStaffID { get; set; }
            public string DeliveryStaffID { get; set; }
            public string Status { get; set; }
        }
    }
}