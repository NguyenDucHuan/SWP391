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
using DiamondShopServices.OrderServices;
using DiamondShopServices;
using System.Data.Entity.Infrastructure;
using DiamondShopDAOs.CookieCartDAO;

namespace ProjectDiamondShop.Controllers
{
    public class ManagerController : Controller
    {
        private readonly IManagerService _managerService;
        private readonly IUserService _userService;
        private readonly INotificationService _notificationService;
        private readonly IWarrantyService _warrantyService;
        private readonly IDiamondService _diamondService;
        private readonly IOrderServices _orderServices;

        public ManagerController()
        {
            _managerService = new ManagerService();
            _warrantyService = new WarrantyService();
            _diamondService = new DiamondService();
            _orderServices = new OrderServices();
            _notificationService = new NotificationService();
            _userService = new UserService();
        }

        public ActionResult Index(string searchQuery = "", int page = 1, int pageSize = 10, int orderPageSize = 10, int voucherPageSize = 10,
            int accentStonePage = 1, int accentStonePageSize = 10, int settingPageSize = 10, int saleStaffPageSize = 10,
            int deliveryStaffPageSize = 10, int userPageSize = 10, int notificationPageSize = 10, int warrantyPage = 1, int warrantyPageSize = 10)
        {
            if (Session["RoleID"] == null || ((int)Session["RoleID"] != 2 && (int)Session["RoleID"] != 3))
            {
                return RedirectToAction("Index", "Home");
            }
            ViewBag.userName = Session["UserName"];
            ViewBag.roleName = Session["RoleName"];

            var orders = _managerService.GetOrders();
            var vouchers = _managerService.GetVouchers();
            var accentStones = _managerService.GetAccentStones();
            var settings = _managerService.GetSettings();
            var saleStaff = _managerService.GetUsersByRole(5);
            var deliveryStaff = _managerService.GetUsersByRole(4);
            var users = _managerService.GetUsers();
            var diamonds = _managerService.GetDiamonds();
            var userId = Session["UserID"].ToString();
            var notifications = _notificationService.GetAllNotifications().Where(n => n.userID == userId).ToList();
            var warranties = _managerService.GetWarranties().Where(w => w.status != "Valid").ToList();

            // Tìm kiếm
            if (!string.IsNullOrEmpty(searchQuery))
            {
                orders = orders.Where(o => o.customerName.Contains(searchQuery)).ToList(); // Thay customerName bằng tên thuộc tính bạn muốn tìm kiếm
                vouchers = vouchers.Where(v => v.targetUserID.Contains(searchQuery)).ToList(); // Thay targetUserID bằng tên thuộc tính bạn muốn tìm kiếm
                accentStones = accentStones.Where(a => a.accentStonesName.Contains(searchQuery)).ToList();
                settings = settings.Where(s => s.settingType.Contains(searchQuery)).ToList(); // Thay settingType bằng tên thuộc tính bạn muốn tìm kiếm
                saleStaff = saleStaff.Where(s => s.userName.Contains(searchQuery)).ToList(); // Thay userName bằng tên thuộc tính bạn muốn tìm kiếm
                deliveryStaff = deliveryStaff.Where(d => d.userName.Contains(searchQuery)).ToList(); // Thay userName bằng tên thuộc tính bạn muốn tìm kiếm
                users = users.Where(u => u.userName.Contains(searchQuery)).ToList(); // Thay userName bằng tên thuộc tính bạn muốn tìm kiếm
                diamonds = diamonds.Where(d => d.diamondName.Contains(searchQuery)).ToList();
                notifications = notifications.Where(n => n.detail.Contains(searchQuery)).ToList(); // Thay detail bằng tên thuộc tính bạn muốn tìm kiếm
                warranties = warranties.Where(w => w.warrantyDetails.Contains(searchQuery)).ToList(); // Thay warrantyDetails bằng tên thuộc tính bạn muốn tìm kiếm
            }

            // Phân trang
            ViewBag.TotalOrders = orders.Count();
            ViewBag.OrderPageSize = orderPageSize;
            ViewBag.CurrentOrderPage = page;
            ViewBag.Orders = orders.Skip((page - 1) * orderPageSize).Take(orderPageSize).ToList();

            ViewBag.TotalVouchers = vouchers.Count();
            ViewBag.VoucherPageSize = voucherPageSize;
            ViewBag.CurrentVoucherPage = page;
            ViewBag.Vouchers = vouchers.Skip((page - 1) * voucherPageSize).Take(voucherPageSize).ToList();

            ViewBag.TotalAccentStones = accentStones.Count();
            ViewBag.AccentStonePageSize = accentStonePageSize;
            ViewBag.CurrentAccentStonePage = accentStonePage;
            ViewBag.AccentStones = accentStones.Skip((accentStonePage - 1) * accentStonePageSize).Take(accentStonePageSize).ToList();

            ViewBag.TotalSettings = settings.Count();
            ViewBag.SettingPageSize = settingPageSize;
            ViewBag.CurrentSettingPage = page;
            ViewBag.Settings = settings.Skip((page - 1) * settingPageSize).Take(settingPageSize).ToList();

            ViewBag.TotalSaleStaff = saleStaff.Count();
            ViewBag.SaleStaffPageSize = saleStaffPageSize;
            ViewBag.CurrentSaleStaffPage = page;
            ViewBag.SaleStaff = saleStaff.Skip((page - 1) * saleStaffPageSize).Take(saleStaffPageSize).ToList();

            ViewBag.TotalDeliveryStaff = deliveryStaff.Count();
            ViewBag.DeliveryStaffPageSize = deliveryStaffPageSize;
            ViewBag.CurrentDeliveryStaffPage = page;
            ViewBag.DeliveryStaff = deliveryStaff.Skip((page - 1) * deliveryStaffPageSize).Take(deliveryStaffPageSize).ToList();

            ViewBag.TotalUsers = users.Count();
            ViewBag.UserPageSize = userPageSize;
            ViewBag.CurrentUserPage = page;
            ViewBag.Users = users.Skip((page - 1) * userPageSize).Take(userPageSize).ToList();

            ViewBag.TotalDiamonds = diamonds.Count();
            ViewBag.PageSize = pageSize;
            ViewBag.CurrentPage = page;
            ViewBag.Diamonds = diamonds.Skip((page - 1) * pageSize).Take(pageSize).ToList();

            ViewBag.TotalNotifications = notifications.Count();
            ViewBag.NotificationPageSize = notificationPageSize;
            ViewBag.CurrentNotificationPage = page;
            ViewBag.Notifications = notifications.Skip((page - 1) * notificationPageSize).Take(notificationPageSize).ToList();

            var revenueData = _managerService.GetRevenueData();
            ViewBag.RevenueLabels = revenueData.Select(r => r.Date).ToList();
            ViewBag.RevenueData = revenueData.Select(r => r.Revenue).ToList();

            if ((int)Session["RoleID"] == 2)
            {
                var registrationData = _managerService.GetRegistrationData();
                ViewBag.RegistrationLabels = registrationData.Select(r => r.Date).ToList();
                ViewBag.RegistrationData = registrationData.Select(r => r.Registrations).ToList();
            }

            ViewBag.TotalWarranties = warranties.Count();
            ViewBag.WarrantyPageSize = warrantyPageSize;
            ViewBag.CurrentWarrantyPage = warrantyPage;
            ViewBag.Warranties = warranties.Skip((warrantyPage - 1) * warrantyPageSize).Take(warrantyPageSize).ToList();

            // Trả về toàn bộ danh sách kim cương
            ViewBag.AllDiamonds = diamonds;

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
                        // Notification
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
        "Delivered"
    };

            var currentIndex = statusOrder.IndexOf(currentStatus);
            var newIndex = statusOrder.IndexOf(newStatus);

            // Special case: Allow transition from "Order Placed" to "Paid"
            if (currentStatus == "Order Placed" && newStatus == "Paid")
            {
                return true;
            }

            // Ensure that status can only move to the next status in the list
            return newIndex > currentIndex && newIndex == currentIndex + 1;
        }


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
            catch (DbEntityValidationException ex)
            {
                var errorMessages = ex.EntityValidationErrors
                    .SelectMany(x => x.ValidationErrors)
                    .Select(x => x.ErrorMessage);

                var fullErrorMessage = string.Join("; ", errorMessages);
                var exceptionMessage = string.Concat(ex.Message, " The validation errors are: ", fullErrorMessage);

                return Json(new { success = false, message = exceptionMessage });
            }
            catch (Exception ex)
            {
                var innerExceptionMessage = ex.InnerException != null ? ex.InnerException.Message : ex.Message;
                return Json(new { success = false, message = "An error occurred while updating the entries: " + innerExceptionMessage });
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
            string userId = GenerateNextUserId();

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
            catch (DbUpdateException ex)
            {
                var innerExceptionMessage = ex.InnerException?.InnerException?.Message ?? ex.InnerException?.Message ?? ex.Message;
                TempData["ErrorMessage"] = "An error occurred while updating the entries: " + innerExceptionMessage;

                // Log the error details to debug output
                System.Diagnostics.Debug.WriteLine("DbUpdateException: " + ex.ToString());
                Console.WriteLine("DbUpdateException: " + ex.ToString());
            }
            catch (DbEntityValidationException ex)
            {
                var errorMessages = ex.EntityValidationErrors
                    .SelectMany(x => x.ValidationErrors)
                    .Select(x => x.ErrorMessage);

                var fullErrorMessage = string.Join("; ", errorMessages);
                TempData["ErrorMessage"] = "Validation failed: " + fullErrorMessage;

                // Log the error details to debug output
                System.Diagnostics.Debug.WriteLine("DbEntityValidationException: " + ex.ToString());
                Console.WriteLine("DbEntityValidationException: " + ex.ToString());
            }
            catch (Exception ex)
            {
                var innerExceptionMessage = ex.InnerException != null ? ex.InnerException.Message : ex.Message;
                TempData["ErrorMessage"] = "An error occurred while updating the entries: " + innerExceptionMessage;

                // Log the error details to debug output
                System.Diagnostics.Debug.WriteLine("Exception: " + ex.ToString());
                Console.WriteLine("Exception: " + ex.ToString());
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

        private string GenerateNextUserId()
        {
            string currentId = _userService.GetTheLastestUserID();
            if (string.IsNullOrEmpty(currentId))
            {
                return "ID0000001";
            }

            string numericPart = currentId.Substring(2);
            if (!int.TryParse(numericPart, out int numericValue))
            {
                throw new ArgumentException("Invalid numeric part in ID");
            }
            numericValue++;
            string newNumericPart = numericValue.ToString().PadLeft(numericPart.Length, '0');
            return "ID" + newNumericPart;
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

            // Check if end time is exactly one day after start time
            if ((endTime - startTime).TotalDays != 1)
            {
                TempData["ErrorMessage"] = "End time must be exactly one day after start time.";
                return RedirectToAction("CreateVoucher");
            }

            // Ensure targetUserID is not null
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
                targetUserID = targetUserID // Set the targetUserID value
            };

            try
            {
                _managerService.AddVoucher(voucher);
                _managerService.SaveChanges();
                TempData["SuccessMessage"] = "Voucher created successfully.";

                // Notification
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
                string folderPath = Server.MapPath("~/Image/Settings");

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
                    imagePath = $"/Image/Settings/{fileName}";
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
                string folderPath = Server.MapPath("~/Image/AccentStones");

                // Check if the folder exists, if not, create it
                if (!Directory.Exists(folderPath))
                {
                    Directory.CreateDirectory(folderPath);
                }

                // Check for required fields
                if (accentStoneImage == null || accentStoneImage.ContentLength == 0 ||
                    string.IsNullOrEmpty(accentStone.accentStonesName) ||
                    string.IsNullOrEmpty(accentStone.shape) ||
                    string.IsNullOrEmpty(accentStone.clarity) ||
                    string.IsNullOrEmpty(accentStone.color) ||
                    accentStone.caratWeight <= 0 ||
                    accentStone.price <= 0 ||
                    accentStone.quantity <= 0)
                {
                    TempData["ErrorMessage"] = "Please fill in all required fields and upload an image.";
                    return RedirectToAction("AddAccentStone");
                }

                // Save accent stone image
                if (accentStoneImage != null && accentStoneImage.ContentLength > 0)
                {
                    string fileName = Path.GetFileName(accentStoneImage.FileName);
                    string path = Path.Combine(folderPath, fileName);
                    accentStoneImage.SaveAs(path);
                    imagePath = $"/Image/AccentStones/{fileName}";
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

            TempData["ErrorMessage"] = "An error occurred. Please try again.";
            return RedirectToAction("AddAccentStone");
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
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult HandleRebuyRequest(int notificationID, bool isAccepted)
        {
            var notification = _notificationService.GetNotificationById(notificationID);
            if (notification == null)
            {
                return HttpNotFound();
            }

            int diamondID = ExtractDiamondIDFromDetail(notification.detail);
            var diamond = _diamondService.GetDiamondById(diamondID);
            if (diamond == null)
            {
                return HttpNotFound();
            }

            string saleStaffID = ExtractSaleStaffIDFromNotification(notification.detail);

            if (isAccepted)
            {
                diamond.status = true;
                diamond.detailStatus = "Rebuy";
                _diamondService.UpdateDiamond(diamond);
                notification.status = false;
                // Gửi thông báo đến Sale Staff
                _notificationService.AddNotification(new tblNotification
                {
                    userID = saleStaffID,
                    date = DateTime.Now,
                    detail = $"Your rebuy request for Diamond {diamondID} has been accepted.",
                    status = true
                });
                TempData["Message"] = "Rebuy request accepted successfully.";
            }
            else
            {
                diamond.status = false;
                diamond.detailStatus = "Sold";
                diamond.rebuyPrice = 0;
                diamond.stillHaveCertification = null;
                _diamondService.UpdateDiamond(diamond);
                notification.status = false;
                // Gửi thông báo đến Sale Staff
                _notificationService.AddNotification(new tblNotification
                {
                    userID = saleStaffID,
                    date = DateTime.Now,
                    detail = $"Your rebuy request for Diamond {diamondID} has been rejected.",
                    status = true
                });
                TempData["Message"] = "Rebuy request rejected.";
            }

            _notificationService.UpdateNotification(notification);
            UpdateRelatedNotifications(saleStaffID, notificationID, diamondID);

            return RedirectToAction("Index");
        }
        private void UpdateRelatedNotifications(string saleStaffID, int currentNotificationID, int diamondID)
        {
            var relatedNotifications = _notificationService.GetAllNotifications()
                .Where(n => n.detail.Contains($"has ID: {saleStaffID}") && n.detail.Contains($"Diamond {diamondID}") && n.notificationID != currentNotificationID)
                .ToList();

            foreach (var relatedNotification in relatedNotifications)
            {
                relatedNotification.status = false;
                _notificationService.UpdateNotification(relatedNotification);
            }
        }

        private int ExtractDiamondIDFromDetail(string detail)
        {
            // Implement logic to extract diamondID from the detail string
            var match = Regex.Match(detail, @"Diamond (\d+)");
            if (match.Success)
            {
                return int.Parse(match.Groups[1].Value);
            }
            throw new InvalidOperationException("DiamondID not found in detail");
        }
        private string ExtractSaleStaffIDFromNotification(string detail)
        {
            var match = Regex.Match(detail, @"has ID: (\w+)");
            if (match.Success)
            {
                return match.Groups[1].Value;
            }
            throw new InvalidOperationException("SaleStaffID not found in detail");
        }

        public ActionResult Notifications()
        {
            if (Session["RoleID"] == null || ((int)Session["RoleID"] != 2 && (int)Session["RoleID"] != 3))
            {
                return RedirectToAction("Index", "Home");
            }

            var userId = Session["UserID"].ToString();
            var notifications = _notificationService.GetAllNotifications()
                .Where(n => n.userID == userId)
                .ToList();

            return View(notifications);
        }
        public ActionResult Warranties()
        {
            if (Session["RoleID"] == null || ((int)Session["RoleID"] != 2 && (int)Session["RoleID"] != 3))
            {
                return RedirectToAction("Index", "Home");
            }

            var warranties = _managerService.GetWarranties();
            return View(warranties);
        }

        public ActionResult AddWarranty()
        {
            if (Session["RoleID"] == null || ((int)Session["RoleID"] != 2 && (int)Session["RoleID"] != 3))
            {
                return RedirectToAction("Index", "Home");
            }

            var orders = _managerService.GetOrders();
            ViewBag.Orders = new SelectList(orders, "orderID", "orderID");
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AddWarranty(tblWarranty warranty)
        {
            if (Session["RoleID"] == null || ((int)Session["RoleID"] != 2 && (int)Session["RoleID"] != 3))
            {
                TempData["ErrorMessage"] = "Unauthorized access.";
                return RedirectToAction("Index");
            }

            if (ModelState.IsValid)
            {
                _managerService.AddWarranty(warranty);
                _managerService.SaveChanges();
                TempData["SuccessMessage"] = "Warranty added successfully!";
                return RedirectToAction("Warranties");
            }

            TempData["ErrorMessage"] = "An error occurred. Please try again.";
            return View(warranty);
        }

        [HttpPost]
        public JsonResult ToggleWarrantyStatus(int warrantyID, bool status)
        {
            try
            {
                _managerService.ToggleWarrantyStatus(warrantyID, status);
                return Json(new { success = true });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }
        public ActionResult LoadData(string type, int page = 1, int pageSize = 10, string sortCriteria = null)
        {
            // Kiểm tra quyền truy cập
            if (Session["RoleID"] == null || ((int)Session["RoleID"] != 2 && (int)Session["RoleID"] != 3))
            {
                return RedirectToAction("Index", "Home");
            }

            ViewBag.userName = Session["UserName"];
            ViewBag.roleName = Session["RoleName"];

            // Lấy danh sách dữ liệu dựa trên type
            IEnumerable<object> data = null;
            switch (type)
            {
                case "orders":
                    data = _managerService.GetOrders();
                    break;
                case "diamonds":
                    data = _managerService.GetDiamonds();
                    break;
                case "warranties":
                    data = _managerService.GetWarranties();
                    break;
                // Thêm các loại khác nếu cần
                default:
                    return RedirectToAction("Index", "Home");
            }

            // Sắp xếp theo tiêu chí
            switch (sortCriteria)
            {
                case "alphaAsc":
                    data = data.OrderBy(d => GetPropertyValue(d, "customerName")).ToList();
                    break;
                case "alphaDesc":
                    data = data.OrderByDescending(d => GetPropertyValue(d, "customerName")).ToList();
                    break;
                case "priceAsc":
                    data = data.OrderBy(d => GetPropertyValue(d, "totalMoney")).ToList();
                    break;
                case "priceDesc":
                    data = data.OrderByDescending(d => GetPropertyValue(d, "totalMoney")).ToList();
                    break;
                case "phoneAsc":
                    data = data.OrderBy(d => GetPropertyValue(d, "phone")).ToList();
                    break;
                case "phoneDesc":
                    data = data.OrderByDescending(d => GetPropertyValue(d, "phone")).ToList();
                    break;
                case "dateAsc":
                    data = data.OrderBy(d => GetPropertyValue(d, "saleDate")).ToList();
                    break;
                case "dateDesc":
                    data = data.OrderByDescending(d => GetPropertyValue(d, "saleDate")).ToList();
                    break;
                default:
                    break;
            }

            // Phân trang
            var totalItems = data.Count();
            data = data.Skip((page - 1) * pageSize).Take(pageSize).ToList();

            // Truyền dữ liệu vào ViewBag
            ViewBag.Data = data;
            ViewBag.TotalItems = totalItems;
            ViewBag.PageSize = pageSize;
            ViewBag.CurrentPage = page;
            ViewBag.Type = type;

            return PartialView("_LoadData", ViewBag);
        }

        private object GetPropertyValue(object obj, string propertyName)
        {
            return obj.GetType().GetProperty(propertyName)?.GetValue(obj, null);
        }

        private string GetUserID()
        {
            return Session["UserID"]?.ToString();
        }
    }
}
