using PayPal;
using PayPal.Api;
using ProjectDiamondShop.Models;
using ProjectDiamondShop.Models.Payments;
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
        private const decimal discountPercentage = 0.8m;
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
                    cmd.Parameters.AddWithValue("@totalMoney", cart.Items.Sum(i => i.diamondPrice));
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
                        cmdItem.Parameters.AddWithValue("@diamondID", item.diamondID);
                        cmdItem.Parameters.AddWithValue("@salePrice", item.diamondPrice);
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
        private Models.Order GetOrderDetails(string orderId)
        {
            Models.Order order = null;

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand("SELECT orderID, customerID, deliveryStaffID, saleStaffID, totalMoney, status, address, phone, saleDate FROM tblOrder WHERE orderID = @OrderID", conn);
                cmd.Parameters.AddWithValue("@OrderID", orderId);
                SqlDataReader reader = cmd.ExecuteReader();
                if (reader.Read())
                {
                    order = new Models.Order
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

        private Models.Order GetOrderDetail(string orderId)
        {
            Models.Order order = null;

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand("SELECT orderID, customerID, deliveryStaffID, saleStaffID, totalMoney, status, address, phone, saleDate FROM tblOrder WHERE orderID = @OrderID", conn);
                cmd.Parameters.AddWithValue("@OrderID", orderId);
                SqlDataReader reader = cmd.ExecuteReader();
                if (reader.Read())
                {
                    order = new Models.Order
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
            //getting the apiContext  
            APIContext apiContext = PaypalConfiguration.GetAPIContext();
            try
            {
                //A resource representing a Payer that funds a payment Payment Method as paypal  
                //Payer Id will be returned when payment proceeds or click to pay  
                string payerId = Request.Params["PayerID"];
                if (string.IsNullOrEmpty(payerId))
                {
                    //this section will be executed first because PayerID doesn't exist  
                    //it is returned by the create function call of the payment class  
                    // Creating a payment  
                    // baseURL is the url on which paypal sendsback the data.  
                    string baseURI = Request.Url.Scheme + "://" + Request.Url.Authority + "/order/PaymentWithPayPal?";
                    //here we are generating guid for storing the paymentID received in session  
                    //which will be used in the payment execution  
                    var guid = Convert.ToString((new Random()).Next(100000));
                    //CreatePayment function gives us the payment approval url  
                    //on which payer is redirected for paypal account payment  
                    var createdPayment = this.CreatePayment(apiContext, baseURI + "guid=" + guid);
                    //get links returned from paypal in response to Create function call  
                    var links = createdPayment.links.GetEnumerator();
                    string paypalRedirectUrl = null;
                    while (links.MoveNext())
                    {
                        Links lnk = links.Current;
                        if (lnk.rel.ToLower().Trim().Equals("approval_url"))
                        {
                            //saving the payapalredirect URL to which user will be redirected for payment  
                            paypalRedirectUrl = lnk.href;
                        }
                    }
                    // saving the paymentID in the key guid  
                    Session.Add(guid, createdPayment.id);
                    return Redirect(paypalRedirectUrl);
                }
                else
                {
                    // This function exectues after receving all parameters for the payment  
                    var guid = Request.Params["guid"];
                    var executedPayment = ExecutePayment(apiContext, payerId, Session[guid] as string);
                    //If executed payment failed then we will show payment failure message to user  
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
            //on successful payment, show success page to user.  
            var userID = GetUserID();
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
                    cmd.Parameters.AddWithValue("@totalMoney", cart.Items.Sum(i => i.diamondPrice));
                    cmd.Parameters.AddWithValue("@status", DEFAULT_ORDER_STATUS);
                    cmd.Parameters.AddWithValue("@address", address);
                    cmd.Parameters.AddWithValue("@phone", phone);
                    cmd.Parameters.AddWithValue("@saleDate", DateTime.Now);
                    cmd.ExecuteNonQuery();
                    foreach (var item in cart.Items)
                    {
                        SqlCommand cmdItem = new SqlCommand("INSERT INTO tblOrderItem (orderID, diamondID, salePrice) VALUES (@orderID, @diamondID, @salePrice)", conn, transaction);
                        cmdItem.Parameters.AddWithValue("@orderID", orderId);
                        cmdItem.Parameters.AddWithValue("@diamondID", item.diamondID);
                        cmdItem.Parameters.AddWithValue("@salePrice", item.diamondPrice);
                        cmdItem.ExecuteNonQuery();
                    }

                    transaction.Commit();

                    // Xóa giỏ hàng sau khi tạo đơn hàng thành công
                    CartHelper.ClearCart(HttpContext, userID);
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    ModelState.AddModelError("", "Error occurred while saving order: " + ex.Message);
                }
            }
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
            decimal totalMoney = 0;
            var itemList = new ItemList()
            {
                items = new List<Item>()
            };

            foreach (var item in cart.Items)
            {
                decimal discountedPrice = item.diamondPrice * (1 - discountPercentage);
                itemList.items.Add(new Item()
                {
                    name = item.DiamondName,
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

            decimal discountedSubtotal = cart.Items.Sum(i => i.diamondPrice * (1 - discountPercentage));

            var details = new Details()
            {
                tax = "0.00",
                shipping = "0.00",
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

                throw;
            }
        }
    }
}
