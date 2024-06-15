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
        private const string DEFAULT_ORDER_STATUS = "Order Placed";
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
                tblOrder newOrder = orderServices.CreateOrder(userID, totalMoney, paidAmount, remainingAmount, address, phone, DEFAULT_ORDER_STATUS);
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

        public ActionResult UpdateOrderDetails(string orderId)
        {
            var userID = GetUserID();
            if (string.IsNullOrEmpty(userID))
            {
                return RedirectToAction("Index", "Login");
            }

            if (string.IsNullOrEmpty(orderId))
            {
                TempData["ErrorMessage"] = "Order ID is required";
                return RedirectToAction("Index", "Home");
            }

            var order = db.tblOrders.SingleOrDefault(o => o.orderID == orderId);

            // Check if the order exists and if the current user is the customer, sale staff, or delivery staff of the order
            if (order == null ||
                (order.customerID != userID &&
                order.saleStaffID != userID &&
                order.deliveryStaffID != userID))
            {
                TempData["ErrorMessage"] = "You are not authorized to view this order";
                return RedirectToAction("Index", "Home");
            }

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

                tblOrder newOrder = orderServices.CreateOrder(userID, totalMoney, paidAmount, remainingAmount, address, phone, DEFAULT_ORDER_STATUS);
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
