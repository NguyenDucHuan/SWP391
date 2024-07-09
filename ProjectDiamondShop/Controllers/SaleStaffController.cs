using DiamondShopBOs;
using DiamondShopServices;
using DiamondShopServices.NotificationService;
using DiamondShopServices.StaffServices;
using DiamondShopServices.UserService;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace ProjectDiamondShop.Controllers
{
    public class SaleStaffController : Controller
    {
        private readonly IStaffService _staffService;
        private readonly INotificationService _notificationService;
        private readonly IDiamondService _diamondService;
        private readonly IUserService _userService;

        public SaleStaffController()
        {
            _staffService = new StaffService();
            _notificationService = new NotificationService();
            _diamondService = new DiamondService();
            _userService = new UserService();
        }

        private bool IsSaleStaff()
        {
            return Session["RoleID"] != null && (int)Session["RoleID"] == 5;
        }

        private string GetUserID()
        {
            if (Session["UserID"] == null)
            {
                Session["ReturnUrl"] = Url.Action("Index", "SaleStaff");
                return null;
            }
            return Session["UserID"].ToString();
        }

        private ActionResult RedirectToPreviousPage()
        {
            if (Request.UrlReferrer != null)
            {
                return Redirect(Request.UrlReferrer.ToString());
            }
            else
            {
                return RedirectToAction("Index", "Home");
            }
        }

        // GET: SaleStaff
        public ActionResult Index(string searchOrderId, int page = 1, int pageSize = 10)
        {
            if (!IsSaleStaff())
            {
                return RedirectToPreviousPage();
            }

            string saleStaffID = GetUserID();
            if (string.IsNullOrEmpty(saleStaffID))
            {
                return RedirectToAction("Index", "Account");
            }

            List<tblOrder> orders = string.IsNullOrEmpty(searchOrderId) ?
                _staffService.GetOrdersByStaffId(saleStaffID, 5, null) :
                _staffService.GetOrdersByStaffId(saleStaffID, 5, searchOrderId);

            int totalOrders = orders.Count;
            orders = orders.Skip((page - 1) * pageSize).Take(pageSize).ToList();

            ViewBag.Orders = orders;
            ViewBag.SearchOrderId = searchOrderId;
            ViewBag.Page = page;
            ViewBag.PageSize = pageSize;
            ViewBag.TotalOrders = totalOrders;
            return View("SaleStaff");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Process(string orderId)
        {
            if (!IsSaleStaff())
            {
                return RedirectToPreviousPage();
            }

            if (string.IsNullOrEmpty(orderId))
            {
                TempData["UpdateMessage"] = "Order ID is required.";
                return RedirectToAction("Index");
            }

            var order = _staffService.GetOrderById(orderId);
            if (order != null)
            {
                _staffService.UpdateOrderStatus(orderId, "Preparing Goods");
                TempData["UpdateMessage"] = "Order updated successfully.";
            }
            else
            {
                TempData["UpdateMessage"] = "Order not found.";
            }

            return RedirectToAction("Index");
        }

        // This method displays the initial Rebuy page with the search form
        [HttpGet]
        public ActionResult Rebuy()
        {
            if (!IsSaleStaff())
            {
                return RedirectToPreviousPage();
            }

            return View();
        }

        // This method processes the search and displays the diamond details
        [HttpGet]
        public ActionResult SearchRebuy(string searchTerm)
        {
            if (!IsSaleStaff())
            {
                return RedirectToPreviousPage();
            }

            if (string.IsNullOrEmpty(searchTerm))
            {
                ViewBag.Message = "Please enter a search term.";
                return View("Rebuy");
            }

            var diamond = _diamondService.GetDiamondBySearchTerm(searchTerm);
            if (diamond == null)
            {
                ViewBag.Message = "Diamond not found";
                return View("Rebuy");
            }
            return View("Rebuy", diamond);
        }

        [HttpPost]
        public ActionResult SubmitRebuyRequest(int diamondID, bool? stillHaveCertification, decimal rebuyPrice)
        {
            if (!IsSaleStaff())
            {
                return RedirectToPreviousPage();
            }

            // Gán giá trị mặc định nếu stillHaveCertification là null
            bool certification = stillHaveCertification ?? false;
            string saleStaffID = GetUserID();
            if (string.IsNullOrEmpty(saleStaffID))
            {
                TempData["Message"] = "Failed to submit rebuy request. Sale staff information is missing.";
                return RedirectToAction("Rebuy");
            }
            var saleStaff = _userService.GetUserById(saleStaffID);
            if (saleStaff == null)
            {
                TempData["Message"] = "Failed to submit rebuy request. Sale staff information is missing.";
                return RedirectToAction("Rebuy");
            }
            string saleStaffName = saleStaff.fullName;

            // Logic xử lý yêu cầu rebuy
            var diamond = _diamondService.GetDiamondByID(diamondID, false, "Sold");
            if (diamond != null)
            {
                diamond.stillHaveCertification = certification;
                diamond.rebuyPrice = rebuyPrice;
                _diamondService.UpdateDiamond(diamond);

                // Thêm thông báo cho Manager và Admin
                var managersAndAdmins = _userService.GetUsersByRole(new List<int> { 2, 3 }); // 2: Admin, 3: Manager
                foreach (var user in managersAndAdmins)
                {
                    _notificationService.AddNotification(new tblNotification
                    {
                        userID = user.userID,
                        date = DateTime.Now,
                        detail = $"{saleStaffName} has ID: {saleStaffID} rebuy request for Diamond {diamondID} has been submitted. Please review and process.",
                        status = true
                    });
                }

                TempData["Message"] = "Rebuy request submitted successfully.";
            }
            else
            {
                TempData["Message"] = "Failed to submit rebuy request.";
            }

            return RedirectToAction("Rebuy");
        }
    }
}
