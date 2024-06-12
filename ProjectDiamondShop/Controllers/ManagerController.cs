using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text.RegularExpressions;
using System.Text;
using System.Web.Mvc;
using DiamondShopBOs;

namespace ProjectDiamondShop.Controllers
{
    public class ManagerController : Controller
    {
        private DiamondShopManagementEntities db = new DiamondShopManagementEntities();

        public ActionResult Index()
        {
            if (Session["RoleID"] == null || (int)Session["RoleID"] != 2 && (int)Session["RoleID"] != 3)
            {
                return RedirectToAction("Index", "Home");
            }

            ViewBag.Orders = GetOrders();
            ViewBag.Diamonds = GetDiamonds();
            ViewBag.SaleStaff = GetSaleStaff();
            ViewBag.DeliveryStaff = GetDeliveryStaff();
            ViewBag.Users = (int)Session["RoleID"] == 2 ? GetUsers() : null;

            var revenueData = GetRevenueData();
            ViewBag.RevenueLabels = revenueData.Select(r => r.Date).ToList();
            ViewBag.RevenueData = revenueData.Select(r => r.Revenue).ToList();

            if ((int)Session["RoleID"] == 2)
            {
                var registrationData = GetRegistrationData();
                ViewBag.RegistrationLabels = registrationData.Select(r => r.Date).ToList();
                ViewBag.RegistrationData = registrationData.Select(r => r.Registrations).ToList();
            }

            return View();
        }

        private List<tblUser> GetUsers()
        {
            return db.tblUsers.Where(u => u.roleID == 1).ToList();
        }

        private List<RegistrationData> GetRegistrationData()
        {
            var rawData = db.tblUsers
                .Where(u => u.roleID == 1)
                .GroupBy(u => new { u.createDate.Year, u.createDate.Month, u.createDate.Day })
                .Select(g => new
                {
                    Year = g.Key.Year,
                    Month = g.Key.Month,
                    Day = g.Key.Day,
                    Count = g.Count()
                })
                .OrderBy(r => r.Year)
                .ThenBy(r => r.Month)
                .ThenBy(r => r.Day)
                .ToList();

            var registrationData = rawData.Select(x => new RegistrationData
            {
                Date = $"{x.Year}-{x.Month.ToString().PadLeft(2, '0')}-{x.Day.ToString().PadLeft(2, '0')}",
                Registrations = x.Count
            }).ToList();

            return registrationData;
        }




        private List<tblOrder> GetOrders()
        {
            return db.tblOrders.ToList();
        }

        private List<tblDiamond> GetDiamonds()
        {
            return db.tblDiamonds.ToList();
        }

        private List<tblUser> GetSaleStaff()
        {
            return GetUsersByRole(5);
        }

        private List<tblUser> GetDeliveryStaff()
        {
            return GetUsersByRole(4);
        }

        private List<tblUser> GetUsersByRole(int roleID)
        {
            return db.tblUsers.Where(u => u.roleID == roleID).ToList();
        }

        private List<RevenueData> GetRevenueData()
        {
            var data = db.tblOrders
                .Where(o => o.saleDate != null)
                .GroupBy(o => System.Data.Entity.DbFunctions.TruncateTime(o.saleDate))
                .Select(g => new
                {
                    Date = g.Key,
                    Revenue = g.Sum(o => (double?)o.totalMoney) ?? 0
                })
                .OrderBy(r => r.Date)
                .ToList()
                .Select(x => new RevenueData
                {
                    Date = x.Date.Value.ToString("yyyy-MM-dd"),
                    Revenue = x.Revenue
                }).ToList();

            return data;
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
                var order = db.tblOrders.FirstOrDefault(o => o.orderID == update.OrderID);
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

                        db.tblOrderStatusUpdates.Add(new tblOrderStatusUpdate
                        {
                            orderID = order.orderID,
                            status = update.Status,
                            updateTime = DateTime.Now
                        });
                    }
                }
            }

            db.SaveChanges();

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
                var user = db.tblUsers.FirstOrDefault(u => u.userID == userId);
                if (user != null)
                {
                    user.status = status;
                    db.SaveChanges();
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
                return RedirectToAction("Index", "Home");
            }

            if (roleId != 3 && roleId != 4 && roleId != 5)
            {
                ModelState.AddModelError("", "Invalid role selected.");
                return View();
            }

            bool hasErrors = false;

            if (string.IsNullOrEmpty(userName) || string.IsNullOrEmpty(fullName) || string.IsNullOrEmpty(email))
            {
                ModelState.AddModelError("RequiredFields", "All fields are required.");
                hasErrors = true;
            }

            if (string.IsNullOrEmpty(password) || string.IsNullOrEmpty(confirmPassword))
            {
                ModelState.AddModelError("PasswordRequired", "Password and Confirm Password are required.");
                hasErrors = true;
            }
            else if (password != confirmPassword)
            {
                ModelState.AddModelError("PasswordMismatch", "Passwords do not match.");
                hasErrors = true;
            }
            else
            {
                if (!Regex.IsMatch(password, @"[a-z]"))
                {
                    ModelState.AddModelError("PasswordLowercase", "Password must contain at least one lowercase letter.");
                    hasErrors = true;
                }

                if (!Regex.IsMatch(password, @"[A-Z]"))
                {
                    ModelState.AddModelError("PasswordUppercase", "Password must contain at least one uppercase letter.");
                    hasErrors = true;
                }

                if (!Regex.IsMatch(password, @"[\W_]"))
                {
                    ModelState.AddModelError("PasswordSpecialChar", "Password must contain at least one special character.");
                    hasErrors = true;
                }
            }

            string hashedUserName = HashString(userName);
            if (db.tblUsers.Any(u => u.userName == hashedUserName))
            {
                ModelState.AddModelError("DuplicateUserName", "User name already exists.");
                hasErrors = true;
            }

            if (db.tblUsers.Any(u => u.email == email))
            {
                ModelState.AddModelError("DuplicateEmail", "Email already exists.");
                hasErrors = true;
            }

            if (hasErrors)
            {
                ViewBag.UserName = userName;
                ViewBag.FullName = fullName;
                ViewBag.Email = email;
                ViewBag.RoleID = roleId;

                var prioritizedErrors = new[] { "RequiredFields", "PasswordRequired", "PasswordMismatch", "PasswordLowercase", "PasswordUppercase", "PasswordSpecialChar", "DuplicateUserName", "DuplicateEmail" };
                foreach (var key in prioritizedErrors)
                {
                    if (ModelState.ContainsKey(key) && ModelState[key].Errors.Count > 0)
                    {
                        ModelState.AddModelError("", ModelState[key].Errors[0].ErrorMessage);
                        break;
                    }
                }
                return View();
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

            db.tblUsers.Add(newUser);
            db.SaveChanges();

            return RedirectToAction("Index", "Manager");
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

        public class RevenueData
        {
            public string Date { get; set; }
            public double Revenue { get; set; }
        }

        public class RegistrationData
        {
            public string Date { get; set; }
            public int Registrations { get; set; }
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
