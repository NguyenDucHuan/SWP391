using DiamondShopBOs;
using DiamondShopServices.OrderServices;
using PayPal;
using PayPal.Api;
using ProjectDiamondShop.Models;
using ProjectDiamondShop.Models.Payments;
using ProjectDiamondShop.Repositories;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
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
        private const decimal discountPercentage = 0.8m;
        private readonly IOrderServices orderServices = null;
        private readonly IItemService itemService = null;
        private readonly DiamondShopManagementEntities db = new DiamondShopManagementEntities(); // Entity Framework DbContext

        public OrderController()
        {
            if (orderServices == null)
            {
                orderServices = new OrderServices();
            }
            if (itemService == null)
            {
                itemService = new ItemService();
            }
        }
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
        public ActionResult SaveOrder(string address, string phone)
        {
            var userID = GetUserID();
            if (string.IsNullOrEmpty(userID))
            {
                return RedirectToAction("Index", "Login");
            }
            var cart = CartHelper.GetCart(HttpContext, userID);
            decimal totalMoney = cart.Items.Sum(i => i.diamondPrice);
            decimal paidAmount = totalMoney * discountPercentage;
            decimal remainingAmount = totalMoney - paidAmount;

            // Lấy ID của nhân viên giao hàng có RoleID = 4
            var deliveryStaffID = orderServices.GetDeliveryStaffID();

            try
            {
                tblOrder newOrder = orderServices.CreateOrder(userID, totalMoney, paidAmount, remainingAmount, address, phone, DEFAULT_ORDER_STATUS, deliveryStaffID);
                foreach (var item in cart.Items)
                {
                    itemService.CreateItem(newOrder.orderID, item.settingID, item.accentStoneID, item.quantityAccent, item.diamondID, item.diamondPrice, (decimal)item.settingPrice, (decimal)item.accentStonePrice);
                }

                // Clear the cart after order creation
                CartHelper.ClearCart(HttpContext, userID);

                return RedirectToAction("Index", "Diamonds");
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "Error occurred while saving order: " + ex.Message);
                return View("CreateOrder", cart);
            }
        }

        //public ActionResult UpdateOrderDetails(string orderId)
        //{
        //    if (string.IsNullOrEmpty(orderId))
        //    {
        //        return new HttpStatusCodeResult(HttpStatusCode.BadRequest, "Order ID is required");
        //    }

        //    var order = GetOrderDetails(orderId);
        //    var orderItems = GetOrderItems(orderId); // Lấy danh sách các item trong đơn hàng
        //    var statusUpdates = GetStatusUpdates(orderId); // Lấy danh sách cập nhật trạng thái

        //    ViewBag.StatusUpdates = statusUpdates; // Gán giá trị cho ViewBag.StatusUpdates

        //    var viewModel = new ViewOrderViewModel
        //    {
        //        Order = order,
        //        Items = orderItems,
        //        StatusUpdates = statusUpdates
        //    };

        //    return View("UpdateOrderDetails", viewModel);
        //}

        //    [HttpPost]
        //    public ActionResult UpdateStatus(string orderId, string status)
        //    {
        //        if (string.IsNullOrEmpty(orderId) || string.IsNullOrEmpty(status))
        //        {
        //            return new HttpStatusCodeResult(HttpStatusCode.BadRequest, "Order ID and Status are required");
        //        }

        //        var currentStatus = GetCurrentOrderStatus(orderId);

        //        // Define valid status transitions
        //        var validTransitions = new Dictionary<string, List<string>>
        //{
        //    { "Order Placed", new List<string> { "Preparing Goods" } },
        //    { "Preparing Goods", new List<string> { "Shipped to Carrier" } },
        //    { "Shipped to Carrier", new List<string> { "In Delivery" } },
        //    { "In Delivery", new List<string> { "Delivered" } },
        //    { "Delivered", new List<string> { "Paid" } }
        //};

        //        // Check if the transition is valid
        //        if (!validTransitions.ContainsKey(currentStatus) || !validTransitions[currentStatus].Contains(status))
        //        {
        //            TempData["UpdateMessage"] = "Update Error";
        //            return RedirectToAction("UpdateOrderDetails", new { orderId });
        //        }

        //        UpdateOrderStatus(orderId, status);
        //        TempData["UpdateMessage"] = "Update Successful";
        //        return RedirectToAction("UpdateOrderDetails", new { orderId });
        //    }

        //private string GetCurrentOrderStatus(string orderId)
        //{
        //    using (var conn = new SqlConnection(connectionString))
        //    {
        //        conn.Open();
        //        var cmd = new SqlCommand("SELECT status FROM tblOrder WHERE orderID = @OrderID", conn);
        //        cmd.Parameters.AddWithValue("@OrderID", orderId);
        //        return cmd.ExecuteScalar()?.ToString();
        //    }
        //}
        //private Models.Order GetOrderDetails(string orderId)
        //{
        //    Models.Order order = null;

        //    using (SqlConnection conn = new SqlConnection(connectionString))
        //    {
        //        conn.Open();
        //        SqlCommand cmd = new SqlCommand("SELECT orderID, customerID, deliveryStaffID, saleStaffID, totalMoney, status, address, phone, saleDate FROM tblOrder WHERE orderID = @OrderID", conn);
        //        cmd.Parameters.AddWithValue("@OrderID", orderId);
        //        SqlDataReader reader = cmd.ExecuteReader();
        //        if (reader.Read())
        //        {
        //            order = new Models.Order
        //            {
        //                OrderID = reader["orderID"].ToString(),
        //                CustomerID = reader["customerID"].ToString(),
        //                DeliveryStaffID = reader["deliveryStaffID"].ToString(),
        //                SaleStaffID = reader["saleStaffID"].ToString(),
        //                TotalMoney = Convert.ToDouble(reader["totalMoney"]),
        //                Status = reader["status"].ToString(),
        //                Address = reader["address"].ToString(),
        //                Phone = reader["phone"].ToString(),
        //                SaleDate = Convert.ToDateTime(reader["saleDate"])
        //            };
        //        }
        //    }

        //    return order;
        //}

        //private List<CartItem> GetOrderItems(string orderId)
        //{
        //    var items = new List<CartItem>();

        //    using (var conn = new SqlConnection(connectionString))
        //    {
        //        conn.Open();
        //        var cmd = new SqlCommand("SELECT diamondID, salePrice FROM tblOrderItem WHERE orderID = @OrderID", conn);
        //        cmd.Parameters.AddWithValue("@OrderID", orderId);

        //        using (var reader = cmd.ExecuteReader())
        //        {
        //            while (reader.Read())
        //            {
        //                items.Add(new CartItem
        //                {
        //                    DiamondID = Convert.ToInt32(reader["diamondID"]),
        //                    DiamondPrice = Convert.ToDecimal(reader["salePrice"])
        //                });
        //            }
        //        }
        //    }

        //    return items;
        //}

        //private void UpdateOrderStatus(string orderId, string status)
        //{
        //    using (SqlConnection conn = new SqlConnection(connectionString))
        //    {
        //        conn.Open();
        //        SqlCommand cmd = new SqlCommand("UPDATE tblOrder SET status = @Status WHERE orderID = @OrderID", conn);
        //        cmd.Parameters.AddWithValue("@Status", status);
        //        cmd.Parameters.AddWithValue("@OrderID", orderId);
        //        cmd.ExecuteNonQuery();

        //        // Insert into tblOrderStatusUpdates
        //        SqlCommand cmdInsert = new SqlCommand("INSERT INTO tblOrderStatusUpdates (orderID, status, updateTime) VALUES (@OrderID, @Status, @UpdateTime)", conn);
        //        cmdInsert.Parameters.AddWithValue("@OrderID", orderId);
        //        cmdInsert.Parameters.AddWithValue("@Status", status);
        //        cmdInsert.Parameters.AddWithValue("@UpdateTime", DateTime.Now);
        //        cmdInsert.ExecuteNonQuery();
        //    }
        //}

        //private List<KeyValuePair<string, DateTime>> GetStatusUpdates(string orderId)
        //{
        //    var updates = new List<KeyValuePair<string, DateTime>>();

        //    using (var conn = new SqlConnection(connectionString))
        //    {
        //        conn.Open();
        //        var cmd = new SqlCommand("SELECT status, updateTime FROM tblOrderStatusUpdates WHERE orderID = @OrderID ORDER BY updateTime", conn);
        //        cmd.Parameters.AddWithValue("@OrderID", orderId);

        //        using (var reader = cmd.ExecuteReader())
        //        {
        //            while (reader.Read())
        //            {
        //                updates.Add(new KeyValuePair<string, DateTime>(reader["status"].ToString(), Convert.ToDateTime(reader["updateTime"])));
        //            }
        //        }
        //    }

        //    return updates;
        //}

        public ActionResult UpdateOrderDetails(string orderId)
        {
            if (string.IsNullOrEmpty(orderId))
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest, "Order ID is required");
            }

            var order = db.tblOrders.SingleOrDefault(o => o.orderID == orderId);
            var orderItems = db.tblOrderItems.Where(oi => oi.orderID == orderId).ToList();
            var statusUpdates = db.tblOrderStatusUpdates.Where(su => su.orderID == orderId).OrderBy(su => su.updateTime).ToList();

            ViewBag.Order = order;
            ViewBag.Items = orderItems;
            ViewBag.StatusUpdates = statusUpdates;

            return View("UpdateOrderDetails");
        }

        [HttpPost]
        public ActionResult UpdateStatus(string orderId, string status)
        {
            if (Session["RoleID"] == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.Unauthorized, "You are not authorized to update the status");
            }

            int roleId = (int)Session["RoleID"];
            if (roleId != 4 && roleId != 5)
            {
                TempData["UpdateMessage"] = "You are not authorized to update the status";
                return RedirectToAction("UpdateOrderDetails", new { orderId });
            }

            if (string.IsNullOrEmpty(orderId) || string.IsNullOrEmpty(status))
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest, "Order ID and Status are required");
            }

            var order = db.tblOrders.SingleOrDefault(o => o.orderID == orderId);
            if (order == null)
            {
                TempData["UpdateMessage"] = "Order not found.";
                return RedirectToAction("UpdateOrderDetails", new { orderId });
            }

            var currentStatus = order.status;

            // Define valid status transitions
            var validTransitions = new Dictionary<string, List<string>>
            {
                { "Order Placed", new List<string> { "Preparing Goods" } },
                { "Preparing Goods", new List<string> { "Shipped to Carrier" } },
                { "Shipped to Carrier", new List<string> { "In Delivery" } },
                { "In Delivery", new List<string> { "Delivered" } },
                { "Delivered", new List<string> { "Paid" } }
            };

            if (!validTransitions.ContainsKey(currentStatus) || !validTransitions[currentStatus].Contains(status))
            {
                TempData["UpdateMessage"] = "Invalid status transition.";
                return RedirectToAction("UpdateOrderDetails", new { orderId });
            }

            // Role-specific status updates
            if (roleId == 4 && (status == "In Delivery" || status == "Delivered"))
            {
                if (!validTransitions[currentStatus].Contains(status))
                {
                    TempData["UpdateMessage"] = "Invalid status transition.";
                    return RedirectToAction("UpdateOrderDetails", new { orderId });
                }
            }
            else if (roleId == 5 && (status == "Order Placed" || status == "Preparing Goods" || status == "Shipped to Carrier" || status == "Paid"))
            {
                if (!validTransitions[currentStatus].Contains(status))
                {
                    TempData["UpdateMessage"] = "Invalid status transition.";
                    return RedirectToAction("UpdateOrderDetails", new { orderId });
                }
            }
            else
            {
                TempData["UpdateMessage"] = "You are not authorized to update to this status.";
                return RedirectToAction("UpdateOrderDetails", new { orderId });
            }

            order.status = status;

            var orderStatusUpdate = new tblOrderStatusUpdate
            {
                orderID = orderId,
                status = status,
                updateTime = DateTime.Now
            };

            db.tblOrderStatusUpdates.Add(orderStatusUpdate);
            db.SaveChanges();

            TempData["UpdateMessage"] = "Order updated successfully.";
            return RedirectToAction("UpdateOrderDetails", new { orderId });
        }

        public ActionResult FailureView()
        {
            return View();
        }
        public ActionResult SuccessView()
        {
            return View();
        }
        public ActionResult PaymentWithPaypal(string address, string phone, string Cancel = null)
        {
            // Store address and phone in session   
            // Getting the APIContext
            APIContext apiContext = PaypalConfiguration.GetAPIContext();
            try
            {
                string payerId = Request.Params["PayerID"];
                if (string.IsNullOrEmpty(payerId))
                {
                    // Generate a new base URL without address and phone parameters
                    string baseURI = Request.Url.Scheme + "://" + Request.Url.Authority + "/order/PaymentWithPayPal?";
                    var guid = Convert.ToString((new Random()).Next(100000));
                    var createdPayment = this.CreatePayment(apiContext, baseURI + "guid=" + guid);

                    // Get the links returned from PayPal in response to the Create function call
                    var links = createdPayment.links.GetEnumerator();
                    string paypalRedirectUrl = null;
                    while (links.MoveNext())
                    {
                        Links lnk = links.Current;
                        if (lnk.rel.ToLower().Trim().Equals("approval_url"))
                        {
                            paypalRedirectUrl = lnk.href;
                        }
                    }

                    // Save the payment ID in the session
                    Session.Add(guid, createdPayment.id);
                    TempData["Address"] = address;
                    TempData["Phone"] = phone;
                    return Redirect(paypalRedirectUrl);
                }
                else
                {
                    // This section is executed when the user is redirected back from PayPal after approving the payment

                    var guid = Request.Params["guid"];
                    var executedPayment = ExecutePayment(apiContext, payerId, Session[guid] as string);
                    if (executedPayment.state.ToLower() != "approved")
                    {
                        return View("FailureView");
                    }
                }
            }
            catch (Exception ex)
            {
                return View("FailureView");
            }
            address = TempData["Address"] as string;
            phone = TempData["Phone"] as string;
            var userID = GetUserID();
            var cart = CartHelper.GetCart(HttpContext, userID);
            decimal totalMoney = cart.Items.Sum(i => ((decimal)i.diamondPrice + ((decimal)i.accentStonePrice * (decimal)i.quantityAccent) + (decimal)i.settingPrice));
            decimal paidAmount = totalMoney * discountPercentage;
            decimal remainingAmount = totalMoney - paidAmount;

            try
            {
                // Retrieve address and phone from session
                var deliveryStaffID = orderServices.GetDeliveryStaffID();

                tblOrder newOrder = orderServices.CreateOrder(userID, totalMoney, paidAmount, remainingAmount, address, phone, DEFAULT_ORDER_STATUS, deliveryStaffID);
                foreach (var item in cart.Items)
                {
                    itemService.CreateItem(newOrder.orderID, item.settingID, item.accentStoneID, item.quantityAccent, item.diamondID, item.diamondPrice, (decimal)item.settingPrice, (decimal)item.accentStonePrice);
                }
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "Error occurred while saving order: " + ex.Message);
            }
            CartHelper.ClearCart(HttpContext, userID);
            return View("SuccessView");
        }




        private PayPal.Api.Payment payment;
        private Payment ExecutePayment(APIContext apiContext, string payerId, string paymentId)
        {
            var paymentExecution = new PaymentExecution()
            {
                payer_id = payerId
            };
            this.payment = new Payment()
            {
                id = paymentId
            };
            return this.payment.Execute(apiContext, paymentExecution);
        }
        private Payment CreatePayment(APIContext apiContext, string redirectUrl)
        {
            // Create item list and add item objects to it  
            var userID = GetUserID();
            var cart = CartHelper.GetCart(HttpContext, userID);
            var itemList = new ItemList()
            {
                items = new List<Item>()
            };

            foreach (var item in cart.Items)
            {
                decimal discountedPrice = (item.diamondPrice + item.settingPrice + item.accentStonePrice * item.quantityAccent) * (1 - discountPercentage);
                itemList.items.Add(new Item()
                {
                    name = item.decription,
                    currency = "USD",
                    price = discountedPrice.ToString("F2"), // Format price to 2 decimal places
                    quantity = "1",
                    sku = item.diamondID.ToString()
                });
            }
            var payer = new Payer()
            {
                payment_method = "paypal"
            };

            var redirUrls = new RedirectUrls()
            {
                cancel_url = redirectUrl + "&Cancel=true",
                return_url = redirectUrl
            };

            decimal discountedSubtotal = cart.ToatalCartMoney() * (1 - discountPercentage);
            var details = new Details()
            {
                subtotal = discountedSubtotal.ToString("F2")
            };

            var amount = new Amount()
            {
                currency = "USD",
                total = discountedSubtotal.ToString("F2"),
                details = details
            };

            var transactionList = new List<Transaction>();

            var paypalOrderId = DateTime.Now.Ticks;
            transactionList.Add(new Transaction()
            {
                description = $"Invoice #{paypalOrderId}",
                invoice_number = paypalOrderId.ToString(), // Generate a unique Invoice No
                amount = amount,
                item_list = itemList
            });

            var payment = new Payment()
            {
                intent = "sale",
                payer = payer,
                transactions = transactionList,
                redirect_urls = redirUrls
            };

            try
            {
                return payment.Create(apiContext);
            }
            catch (PayPalException ex)
            {
                // Log detailed error message
                Console.WriteLine("PayPalException: " + ex.Message);

                // Log details if available
                if (ex.InnerException != null)
                {
                    Console.WriteLine("InnerException: " + ex.InnerException.Message);
                }

                // Log PayPal error response details
                if (ex is PaymentsException pe && pe.Details != null)
                {
                    var error = pe.Details;
                    Console.WriteLine("Name: " + error.name);
                    Console.WriteLine("Message: " + error.message);
                    Console.WriteLine("InformationLink: " + error.information_link);
                    Console.WriteLine("DebugId: " + error.debug_id);
                }

                throw new Exception(ex.Message);
            }
        }
    }
}
