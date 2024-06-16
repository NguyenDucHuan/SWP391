using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Web.Mvc;
using System.Data.Entity.Validation;
using DiamondShopBOs;
using DiamondShopServices.ManagerServices;

namespace ProjectDiamondShop.Controllers
{
    public class ManagerController : Controller
    {
        private readonly IManagerService _managerService;

        public ManagerController()
        {
            _managerService = new ManagerService();
        }

        public ActionResult Index()
        {
            if (Session["RoleID"] == null || ((int)Session["RoleID"] != 2 && (int)Session["RoleID"] != 3))
            {
                return RedirectToAction("Index", "Home");
            }

            ViewBag.Orders = _managerService.GetOrders();
            ViewBag.Diamonds = _managerService.GetDiamonds();
            ViewBag.SaleStaff = _managerService.GetUsersByRole(5);
            ViewBag.DeliveryStaff = _managerService.GetUsersByRole(4);
            ViewBag.Users = (int)Session["RoleID"] == 2 ? _managerService.GetUsers() : null;

            var revenueData = _managerService.GetRevenueData();
            ViewBag.RevenueLabels = revenueData.Select(r => r.Date).ToList();
            ViewBag.RevenueData = revenueData.Select(r => r.Revenue).ToList();

            if ((int)Session["RoleID"] == 2)
            {
                var registrationData = _managerService.GetRegistrationData();
                ViewBag.RegistrationLabels = registrationData.Select(r => r.Date).ToList();
                ViewBag.RegistrationData = registrationData.Select(r => r.Registrations).ToList();
            }

            return View();
        }

        [HttpPost]
        public ActionResult UpdateOrders(List<OrderUpdateModel> orderUpdates)
        {
            if (Session["RoleID"] == null || ((int)Session["RoleID"] != 3 && (int)Session["RoleID"] != 2))
            {
                return Json(new { success = false, message = "Permission Denied." });
            }

            bool isAdmin = (int)Session["RoleID"] == 2;

            foreach (var update in orderUpdates)
            {
                var order = _managerService.GetOrderById(update.OrderID);
                if (order != null)
                {
                    order.saleStaffID = update.SaleStaffID;
                    order.deliveryStaffID = update.DeliveryStaffID;

                    if (!string.IsNullOrEmpty(update.Status))
                    {
                        string currentStatus = order.status;

                        if (!isAdmin && currentStatus != update.Status && !IsValidStatusTransition(currentStatus, update.Status))
                        {
                            return Json(new { success = false, message = $"Update Error: Invalid status transition from {currentStatus} to {update.Status}. Please update again." });
                        }

                        order.status = update.Status;

                        _managerService.AddOrderStatusUpdate(new tblOrderStatusUpdate
                        {
                            orderID = order.orderID,
                            status = update.Status,
                            updateTime = DateTime.Now
                        });
                    }
                }
            }

            _managerService.SaveChanges();

            return Json(new { success = true, message = "Update Successful" });
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

            return newIndex > currentIndex;
        }

        [HttpPost]
        public JsonResult ToggleUserStatus(string userId, bool status)
        {
            try
            {
                var user = _managerService.GetUserById(userId);
                if (user != null)
                {
                    user.status = status;
                    _managerService.SaveChanges();
                }

                return Json(new { success = true });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        public ActionResult CreateStaffAccount()
        {
            if (Session["RoleID"] == null || (int)Session["RoleID"] != 2)
            {
                return RedirectToAction("Index", "Home");
            }
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CreateStaffAccount(string userName, string password, string confirmPassword, string fullName, string email, int roleId)
        {
            if (Session["RoleID"] == null || (int)Session["RoleID"] != 2)
            {
                TempData["ErrorMessage"] = "Unauthorized access.";
                return RedirectToAction("Index");
            }

            if (roleId != 3 && roleId != 4 && roleId != 5)
            {
                TempData["ErrorMessage"] = "Invalid role selected.";
                return RedirectToAction("Index");
            }

            bool hasErrors = false;

            if (string.IsNullOrEmpty(userName) || string.IsNullOrEmpty(fullName) || string.IsNullOrEmpty(email))
            {
                TempData["ErrorMessage"] = "All fields are required.";
                hasErrors = true;
            }

            if (string.IsNullOrEmpty(password) || string.IsNullOrEmpty(confirmPassword))
            {
                TempData["ErrorMessage"] = "Password and Confirm Password are required.";
                hasErrors = true;
            }
            else if (password != confirmPassword)
            {
                TempData["ErrorMessage"] = "Passwords do not match.";
                hasErrors = true;
            }
            else
            {
                if (!Regex.IsMatch(password, @"[a-z]"))
                {
                    TempData["ErrorMessage"] = "Password must contain at least one lowercase letter.";
                    hasErrors = true;
                }

                if (!Regex.IsMatch(password, @"[A-Z]"))
                {
                    TempData["ErrorMessage"] = "Password must contain at least one uppercase letter.";
                    hasErrors = true;
                }

                if (!Regex.IsMatch(password, @"[\W_]"))
                {
                    TempData["ErrorMessage"] = "Password must contain at least one special character.";
                    hasErrors = true;
                }
            }

            string hashedUserName = HashString(userName);
            if (_managerService.GetUsers().Any(u => u.userName == hashedUserName))
            {
                TempData["ErrorMessage"] = "User name already exists.";
                hasErrors = true;
            }

            if (_managerService.GetUsers().Any(u => u.email == email))
            {
                TempData["ErrorMessage"] = "Email already exists.";
                hasErrors = true;
            }

            if (hasErrors)
            {
                TempData["UserName"] = userName;
                TempData["FullName"] = fullName;
                TempData["Email"] = email;
                TempData["RoleID"] = roleId;
                return RedirectToAction("Index");
            }

            string hashedPassword = HashString(password);
            string userId = GenerateRandomUserId();

            var newUser = new tblUser
            {
                userID = userId,
                userName = hashedUserName,
                fullName = fullName,
                email = email,
                password = hashedPassword,
                roleID = roleId,
                status = true,
                resetCode = null
            };

            try
            {
                _managerService.AddUser(newUser);
                _managerService.SaveChanges();
                TempData["SuccessMessage"] = "Account created successfully.";
            }
            catch (DbEntityValidationException ex)
            {
                var errorMessages = ex.EntityValidationErrors
                    .SelectMany(x => x.ValidationErrors)
                    .Select(x => x.ErrorMessage);

                var fullErrorMessage = string.Join("; ", errorMessages);
                TempData["ErrorMessage"] = "Validation failed: " + fullErrorMessage;
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = ex.Message;
            }

            return RedirectToAction("Index");
        }

        private string HashString(string str)
        {
            using (SHA256 sha256 = SHA256.Create())
            {
                byte[] bytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(str));
                StringBuilder builder = new StringBuilder();
                foreach (byte b in bytes)
                {
                    builder.Append(b.ToString("x2"));
                }
                return builder.ToString().Substring(0, 32);
            }
        }

        private string GenerateRandomUserId()
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            var random = new Random();
            return new string(Enumerable.Repeat(chars, 6)
              .Select(s => s[random.Next(s.Length)]).ToArray());
        }
        public ActionResult CreateVoucher()
        {
            if (Session["RoleID"] == null || (int)Session["RoleID"] != 2)
            {
                return RedirectToAction("Index", "Home");
            }

            var users = _managerService.GetUsersByRole(1); // Get users with roleID = 1
            ViewBag.Users = new SelectList(users, "userID", "userID");
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CreateVoucher(DateTime startTime, DateTime endTime, int discount, int quantity, string targetUserID)
        {
            if (Session["RoleID"] == null || (int)Session["RoleID"] != 2)
            {
                TempData["ErrorMessage"] = "Unauthorized access.";
                return RedirectToAction("Index");
            }

            if (endTime <= startTime)
            {
                TempData["ErrorMessage"] = "End time must be after start time.";
                return RedirectToAction("CreateVoucher");
            }

            // Đảm bảo targetUserID không null
            if (string.IsNullOrEmpty(targetUserID))
            {
                TempData["ErrorMessage"] = "The targetUserID field is required.";
                return RedirectToAction("CreateVoucher");
            }

            var voucher = new tblVoucher
            {
                startTime = startTime,
                endTime = endTime,
                discount = discount,
                quantity = quantity,
                status = true,
                targetUserID = targetUserID // Đặt giá trị của targetUserID
            };

            try
            {
                _managerService.AddVoucher(voucher);
                _managerService.SaveChanges();
                TempData["SuccessMessage"] = "Voucher created successfully.";
            }
            catch (DbEntityValidationException ex)
            {
                var errorMessages = ex.EntityValidationErrors
                    .SelectMany(x => x.ValidationErrors)
                    .Select(x => x.ErrorMessage);

                var fullErrorMessage = string.Join("; ", errorMessages);
                TempData["ErrorMessage"] = "Validation failed: " + fullErrorMessage;
            }
            catch (Exception ex)
            {
                var innerExceptionMessage = ex.InnerException != null ? ex.InnerException.Message : ex.Message;
                TempData["ErrorMessage"] = "An error occurred while updating the entries: " + innerExceptionMessage;
            }

            return RedirectToAction("Index");
        }
    }
}
