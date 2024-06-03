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
        private const string DEFAULT_ORDER_STATUS = "Order Place";
        private readonly string connectionString = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;

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

        public ActionResult UpdateOrderDetails(string orderId)
        {
            if (string.IsNullOrEmpty(orderId))
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest, "Order ID is required");
            }

            var order = GetOrderDetails(orderId);
            var orderItems = GetOrderItems(orderId); // Lấy danh sách các item trong đơn hàng
            var statusUpdates = GetStatusUpdates(orderId); // Lấy danh sách cập nhật trạng thái

            ViewBag.StatusUpdates = statusUpdates; // Gán giá trị cho ViewBag.StatusUpdates

            var viewModel = new ViewOrderViewModel
            {
                Order = order,
                Items = orderItems,
                StatusUpdates = statusUpdates
            };

            return View("UpdateOrderDetails", viewModel);
        }

        [HttpPost]
        public ActionResult UpdateStatus(string orderId, string status)
        {
            if (string.IsNullOrEmpty(orderId) || string.IsNullOrEmpty(status))
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest, "Order ID and Status are required");
            }

            var currentStatus = GetCurrentOrderStatus(orderId);

            // Define valid status transitions
            var validTransitions = new Dictionary<string, List<string>>
    {
        { "Order Placed", new List<string> { "Preparing Goods" } },
        { "Preparing Goods", new List<string> { "Shipped to Carrier" } },
        { "Shipped to Carrier", new List<string> { "In Delivery" } },
        { "In Delivery", new List<string> { "Delivered" } },
        { "Delivered", new List<string> { "Paid" } }
    };

            // Check if the transition is valid
            if (!validTransitions.ContainsKey(currentStatus) || !validTransitions[currentStatus].Contains(status))
            {
                TempData["UpdateMessage"] = "Update Error";
                return RedirectToAction("UpdateOrderDetails", new { orderId });
            }

            UpdateOrderStatus(orderId, status);
            TempData["UpdateMessage"] = "Update Successful";
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

        private List<CartItem> GetOrderItems(string orderId)
        {
            var items = new List<CartItem>();

            using (var conn = new SqlConnection(connectionString))
            {
                conn.Open();
                var cmd = new SqlCommand("SELECT diamondID, salePrice FROM tblOrderItem WHERE orderID = @OrderID", conn);
                cmd.Parameters.AddWithValue("@OrderID", orderId);

                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        items.Add(new CartItem
                        {
                            DiamondID = Convert.ToInt32(reader["diamondID"]),
                            DiamondPrice = Convert.ToDecimal(reader["salePrice"])
                        });
                    }
                }
            }

            return items;
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
            var updates = new List<KeyValuePair<string, DateTime>>();

            using (var conn = new SqlConnection(connectionString))
            {
                conn.Open();
                var cmd = new SqlCommand("SELECT status, updateTime FROM tblOrderStatusUpdates WHERE orderID = @OrderID ORDER BY updateTime", conn);
                cmd.Parameters.AddWithValue("@OrderID", orderId);

                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        updates.Add(new KeyValuePair<string, DateTime>(reader["status"].ToString(), Convert.ToDateTime(reader["updateTime"])));
                    }
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
