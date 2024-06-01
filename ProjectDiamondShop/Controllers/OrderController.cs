using ProjectDiamondShop.Models;
using ProjectDiamondShop.Repositories;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;
using System.Net;
using System.Web.Mvc;

namespace ProjectDiamondShop.Controllers
{
    public class OrderController : Controller
    {
        private const string DEFAULT_ORDER_STATUS = "Order Placed";
        private readonly string connectionString = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;
        private static readonly Dictionary<string, string[]> StatusTransitions = new Dictionary<string, string[]>
        {
            { "Order Placed", new[] { "Preparing Goods" } },
            { "Preparing Goods", new[] { "Shipped to Carrier" } },
            { "Shipped to Carrier", new[] { "In Delivery", "Paid" } },
            { "In Delivery", new[] { "Delivered" } },
            { "Delivered", new[] { "Paid" } }
        };

        private string GetUserID()
        {
            if (Session["UserID"] == null)
            {
                Session["ReturnUrl"] = Url.Action("Index", "Diamonds");
                return null;
            }
            return Session["UserID"].ToString();
        }

        [HttpPost]
        public ActionResult CreateOrder()
        {
            var userID = GetUserID();
            if (string.IsNullOrEmpty(userID))
            {
                return RedirectToAction("Index", "Login");
            }
            var cart = CartHelper.GetCart(HttpContext, userID);
            return View("CreateOrder", cart);
        }

        [HttpPost]
        public ActionResult SaveOrder(string address, string phone)
        {
            var userID = GetUserID();
            if (string.IsNullOrEmpty(userID))
            {
                return RedirectToAction("Index", "Login");
            }
            var cart = CartHelper.GetCart(HttpContext, userID);

            string orderId = Guid.NewGuid().ToString();

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();
                SqlTransaction transaction = conn.BeginTransaction();

                try
                {
                    // Lưu thông tin đơn hàng
                    SqlCommand cmd = new SqlCommand("INSERT INTO tblOrder (orderID, customerID, totalMoney, status, address, phone, saleDate, deliveryStaffID, saleStaffID) VALUES (@orderID, @customerID, @totalMoney, @status, @address, @phone, @saleDate, NULL, NULL)", conn, transaction);
                    cmd.Parameters.AddWithValue("@orderID", orderId);
                    cmd.Parameters.AddWithValue("@customerID", userID);
                    cmd.Parameters.AddWithValue("@totalMoney", cart.Items.Sum(i => i.DiamondPrice));
                    cmd.Parameters.AddWithValue("@status", DEFAULT_ORDER_STATUS);
                    cmd.Parameters.AddWithValue("@address", address);
                    cmd.Parameters.AddWithValue("@phone", phone);
                    cmd.Parameters.AddWithValue("@saleDate", DateTime.Now);
                    cmd.ExecuteNonQuery();

                    // Lưu thông tin chi tiết đơn hàng
                    foreach (var item in cart.Items)
                    {
                        SqlCommand cmdItem = new SqlCommand("INSERT INTO tblOrderItem (orderID, diamondID, salePrice) VALUES (@orderID, @diamondID, @salePrice)", conn, transaction);
                        cmdItem.Parameters.AddWithValue("@orderID", orderId);
                        cmdItem.Parameters.AddWithValue("@diamondID", item.DiamondID);
                        cmdItem.Parameters.AddWithValue("@salePrice", item.DiamondPrice);
                        cmdItem.ExecuteNonQuery();
                    }

                    transaction.Commit();

                    // Xóa giỏ hàng sau khi tạo đơn hàng thành công
                    CartHelper.ClearCart(HttpContext, userID);

                    return RedirectToAction("Index", "Diamonds");
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    ModelState.AddModelError("", "Error occurred while saving order: " + ex.Message);
                    return View("CreateOrder", cart);
                }
            }
        }

        [HttpPost]
        public ActionResult UpdateStatus(string orderId, string status)
        {
            var roleId = (int)Session["RoleID"];
            string[] validStatuses;

            if (roleId == 4)
            {
                validStatuses = new[] { "In Delivery", "Delivered" };
            }
            else if (roleId == 5)
            {
                validStatuses = new[] { "Order Placed", "Preparing Goods", "Shipped to Carrier", "Paid" };
            }
            else
            {
                validStatuses = Array.Empty<string>();
            }

            if (!validStatuses.Contains(status))
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest, "Invalid status update for your role.");
            }

            var currentStatus = GetCurrentOrderStatus(orderId);
            if (!StatusTransitions.ContainsKey(currentStatus) || !StatusTransitions[currentStatus].Contains(status))
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest, "Invalid status transition.");
            }

            UpdateOrderStatus(orderId, status);
            return RedirectToAction("UpdateOrderDetails", new { orderId });
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

        public ActionResult UpdateOrderDetails(string orderId)
        {
            if (string.IsNullOrEmpty(orderId))
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest, "Order ID is required");
            }

            var orderDetails = GetOrderDetails(orderId);
            ViewBag.StatusUpdates = GetStatusUpdates(orderId);
            return View(orderDetails);
        }

        private ViewOrderViewModel GetOrderDetails(string orderId)
        {
            var orderDetails = new ViewOrderViewModel();

            using (var conn = new SqlConnection(connectionString))
            {
                conn.Open();

                // Load Order Items
                var cmdOrderItems = new SqlCommand("SELECT diamondID, salePrice FROM tblOrderItem WHERE orderID = @OrderID", conn);
                cmdOrderItems.Parameters.AddWithValue("@OrderID", orderId);
                var readerOrderItems = cmdOrderItems.ExecuteReader();
                while (readerOrderItems.Read())
                {
                    orderDetails.Items.Add(new CartItem
                    {
                        DiamondID = (int)readerOrderItems["diamondID"],
                        DiamondPrice = (decimal)readerOrderItems["salePrice"]
                    });
                }
                readerOrderItems.Close();

                // Load Voucher ID
                var cmdVoucher = new SqlCommand("SELECT voucherID FROM tblVoucherCatch WHERE userID = (SELECT customerID FROM tblOrder WHERE orderID = @OrderID)", conn);
                cmdVoucher.Parameters.AddWithValue("@OrderID", orderId);
                orderDetails.VoucherID = cmdVoucher.ExecuteScalar()?.ToString();

                // Load Order Details
                var cmdOrder = new SqlCommand("SELECT orderID, customerID, deliveryStaffID, saleStaffID, totalMoney, status, address, phone, saleDate FROM tblOrder WHERE orderID = @OrderID", conn);
                cmdOrder.Parameters.AddWithValue("@OrderID", orderId);
                var readerOrder = cmdOrder.ExecuteReader();
                if (readerOrder.Read())
                {
                    orderDetails.Order = new Order
                    {
                        OrderID = readerOrder["orderID"].ToString(),
                        CustomerID = readerOrder["customerID"].ToString(),
                        DeliveryStaffID = readerOrder["deliveryStaffID"].ToString(),
                        SaleStaffID = readerOrder["saleStaffID"].ToString(),
                        TotalMoney = Convert.ToDouble(readerOrder["totalMoney"]),
                        Status = readerOrder["status"].ToString(),
                        Address = readerOrder["address"].ToString(),
                        Phone = readerOrder["phone"].ToString(),
                        SaleDate = Convert.ToDateTime(readerOrder["saleDate"])
                    };
                }
            }

            return orderDetails;
        }

        private List<KeyValuePair<string, DateTime>> GetStatusUpdates(string orderId)
        {
            var updates = new List<KeyValuePair<string, DateTime>>();

            using (var conn = new SqlConnection(connectionString))
            {
                conn.Open();
                var cmd = new SqlCommand("SELECT status, updateTime FROM tblOrderStatusUpdates WHERE orderID = @OrderID ORDER BY updateTime", conn);
                cmd.Parameters.AddWithValue("@OrderID", orderId);
                var reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    updates.Add(new KeyValuePair<string, DateTime>(reader["status"].ToString(), Convert.ToDateTime(reader["updateTime"])));
                }
            }

            return updates;
        }

        //ViewOrder
        public ActionResult UpdateStatusDetail(string orderId)
        {
            if (string.IsNullOrEmpty(orderId))
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest, "Order ID is required");
            }

            var order = GetOrderDetail(orderId);
            return View(order);
        }

        private Order GetOrderDetail(string orderId)
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
    }
}
