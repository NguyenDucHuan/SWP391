using DiamondShopBOs;
using DiamondShopServices.ManagerServices;
using System;
using System.Collections.Generic;
using System.Data.Entity.Validation;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text.RegularExpressions;
using System.Text;
using System.Web;
using System.Web.Mvc;
using DiamondShopServices.NotificationService;
using DiamondShopServices.UserService;
using DiamondShopServices.WarrantyServices;

namespace ProjectDiamondShop.Controllers
{
    public class ManagerController : Controller
    {
        private readonly IManagerService _managerService;
        private readonly IUserService _userService;
        private readonly INotificationService _notificationService;
        private readonly IWarrantyService _warrantyService;

        public ManagerController()
        {
            _managerService = new ManagerService();
            _warrantyService = new WarrantyService();
        }

        public ActionResult Index(int page = 1, int pageSize = 30)
        {
            if (Session["RoleID"] == null || ((int)Session["RoleID"] != 2 && (int)Session["RoleID"] != 3))
            {
                return RedirectToAction("Index", "Home");
            }
            ViewBag.userName = Session["UserName"];
            ViewBag.roleName = Session["RoleName"];

            ViewBag.Orders = _managerService.GetOrders();

            var diamonds = _managerService.GetDiamonds();
            ViewBag.TotalDiamonds = diamonds.Count();
            ViewBag.PageSize = pageSize;
            ViewBag.CurrentPage = page;
            ViewBag.Diamonds = diamonds.Skip((page - 1) * pageSize).Take(pageSize).ToList();

            ViewBag.SaleStaff = _managerService.GetUsersByRole(5);
            ViewBag.DeliveryStaff = _managerService.GetUsersByRole(4);
            ViewBag.AccentStones = _managerService.GetAccentStones();
            ViewBag.Settings = _managerService.GetSettings();
            ViewBag.Users = (int)Session["RoleID"] == 2 ? _managerService.GetUsers() : null;
            ViewBag.Vouchers = _managerService.GetVouchers();

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

                        // Check for valid status transition for both Admin and Manager
                        if (currentStatus != update.Status && !IsValidStatusTransition(currentStatus, update.Status))
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
                        //Notification
                        _notificationService.AddNotification(new tblNotification
                        {
                            userID = order.customerID,
                            date = DateTime.Now,
                            detail = $"Your order status has been updated to {update.Status}.",
                            status = true
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

            // Ensure that status can only move to the next status in the list
            return newIndex > currentIndex && newIndex == currentIndex + 1;
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

        [HttpPost]
        public JsonResult ToggleAccentStoneStatus(int accentStoneId, bool status)
        {
            try
            {
                var accentStone = _managerService.GetAccentStoneById(accentStoneId);
                if (accentStone != null)
                {
                    accentStone.status = status;
                    _managerService.SaveChanges();
                }

                return Json(new { success = true });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        [HttpPost]
        public JsonResult ToggleSettingStatus(int settingId, bool status)
        {
            try
            {
                var setting = _managerService.GetSettingById(settingId);
                if (setting != null)
                {
                    setting.status = status;
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

            if (_managerService.GetUsers().Any(u => u.userName == userName))
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
                userName = userName,
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
            if (Session["RoleID"] == null || (int)Session["RoleID"] != 2 && (int)Session["RoleID"] != 3)
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
            if (Session["RoleID"] == null || (int)Session["RoleID"] != 2 && (int)Session["RoleID"] != 3)
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
                //Notification
                if (voucher.targetUserID == "All")
                {
                    var users = _userService.GetAllUser();
                    foreach (var user in users)
                    {
                        _notificationService.AddNotification(new tblNotification
                        {
                            userID = user.userID,
                            date = DateTime.Now,
                            detail = "A new voucher is available.",
                            status = true
                        });
                    }
                }
                else
                {
                    _notificationService.AddNotification(new tblNotification
                    {
                        userID = voucher.targetUserID,
                        date = DateTime.Now,
                        detail = "You have received a new voucher.",
                        status = true
                    });
                }
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

        public ActionResult AddSetting()
        {
            if (Session["RoleID"] == null || (int)Session["RoleID"] != 3 && (int)Session["RoleID"] != 2)
            {
                return RedirectToAction("Index", "Home");
            }
            return View();
        }

        [HttpPost]
        public ActionResult AddSetting(tblSetting setting, HttpPostedFileBase settingImage)
        {
            if (Session["RoleID"] == null || (int)Session["RoleID"] != 3 && (int)Session["RoleID"] != 2)
            {
                return RedirectToAction("Index", "Home");
            }

            if (ModelState.IsValid)
            {
                string imagePath = "";
                string folderPath = Server.MapPath("~/Images/Settings");

                // Check if the folder exists, if not, create it
                if (!Directory.Exists(folderPath))
                {
                    Directory.CreateDirectory(folderPath);
                }

                // Save setting image
                if (settingImage != null && settingImage.ContentLength > 0)
                {
                    string fileName = Path.GetFileName(settingImage.FileName);
                    string path = Path.Combine(folderPath, fileName);
                    settingImage.SaveAs(path);
                    imagePath = $"/Images/Settings/{fileName}";
                }

                // Create Setting object
                tblSetting newSetting = new tblSetting
                {
                    settingType = setting.settingType,
                    material = setting.material,
                    priceTax = setting.priceTax,
                    quantityStones = setting.quantityStones,
                    description = setting.description,
                    imagePath = imagePath,
                    status = true
                };

                _managerService.AddSetting(newSetting);

                TempData["SuccessMessage"] = "Setting added successfully!";
                return RedirectToAction("AddSetting");
            }

            return View(setting);
        }

        public ActionResult AddAccentStone()
        {
            if (Session["RoleID"] == null || (int)Session["RoleID"] != 3 && (int)Session["RoleID"] != 2)
            {
                return RedirectToAction("Index", "Home");
            }
            return View();
        }

        [HttpPost]
        public ActionResult AddAccentStone(tblAccentStone accentStone, HttpPostedFileBase accentStoneImage)
        {
            if (Session["RoleID"] == null || (int)Session["RoleID"] != 3 && (int)Session["RoleID"] != 2)
            {
                return RedirectToAction("Index", "Home");
            }

            if (ModelState.IsValid)
            {
                string imagePath = "";
                string folderPath = Server.MapPath("~/Images/AccentStones");

                // Check if the folder exists, if not, create it
                if (!Directory.Exists(folderPath))
                {
                    Directory.CreateDirectory(folderPath);
                }

                // Save accent stone image
                if (accentStoneImage != null && accentStoneImage.ContentLength > 0)
                {
                    string fileName = Path.GetFileName(accentStoneImage.FileName);
                    string path = Path.Combine(folderPath, fileName);
                    accentStoneImage.SaveAs(path);
                    imagePath = $"/Images/AccentStones/{fileName}";
                }

                // Create Accent Stone object
                tblAccentStone newAccentStone = new tblAccentStone
                {
                    accentStonesName = accentStone.accentStonesName,
                    shape = accentStone.shape,
                    caratWeight = accentStone.caratWeight,
                    clarity = accentStone.clarity,
                    color = accentStone.color,
                    price = accentStone.price,
                    quantity = accentStone.quantity,
                    imagePath = imagePath,
                    status = true
                };

                _managerService.AddAccentStone(newAccentStone);

                TempData["SuccessMessage"] = "Accent Stone added successfully!";
                return RedirectToAction("AddAccentStone");
            }

            return View(accentStone);
        }

        [HttpGet]
        public JsonResult GetChartData(string viewType, int? month, int? year)
        {
            // Fetch and process data based on viewType, month, and year parameters.
            // This is a placeholder implementation and should be modified to match your actual data retrieval logic.
            var revenueData = _managerService.GetRevenueData(); // Modify this method to accept viewType, month, year
            var registrationData = _managerService.GetRegistrationData(); // Modify this method to accept viewType, month, year

            var response = new
            {
                revenueLabels = revenueData.Select(d => d.Date).ToArray(),
                revenueData = revenueData.Select(d => d.Revenue).ToArray(),
                registrationLabels = registrationData.Select(d => d.Date).ToArray(),
                registrationData = registrationData.Select(d => d.Registrations).ToArray()
            };

            return Json(response, JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        public JsonResult ToggleDiamondStatus(int diamondId, bool status)
        {
            try
            {
                var diamond = _managerService.GetDiamondById(diamondId);
                if (diamond != null)
                {
                    diamond.status = status;
                    _managerService.SaveChanges();
                }

                return Json(new { success = true });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        [HttpPost]
        public JsonResult ToggleVoucherStatus(int voucherId, bool status)
        {
            try
            {
                _managerService.ToggleVoucherStatus(voucherId, status);
                return Json(new { success = true });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }
        public ActionResult UseVoucher(int voucherId)
        {
            try
            {
                var voucher = _managerService.GetVouchers().FirstOrDefault(v => v.voucherID == voucherId);
                if (voucher != null && voucher.quantity > 0)
                {
                    voucher.quantity -= 1;
                    _managerService.UpdateVoucherQuantity(voucherId, voucher.quantity);
                }
                return Json(new { success = true });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

    }
}
