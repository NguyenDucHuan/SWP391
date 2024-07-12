using System;
using System.Linq;
using System.Web.Mvc;
using DiamondShopBOs;
using DiamondShopServices.NotificationService;
using DiamondShopServices.OrderServices;
using DiamondShopServices.WarrantyServices;

namespace ProjectDiamondShop.Controllers
{
    public class WarrantyController : Controller
    {
        private readonly IWarrantyService _warrantyService;
        private readonly INotificationService _notificationService;
        private readonly IOrderServices orderServices = null;

        public WarrantyController()
        {
            _warrantyService = new WarrantyService();
            _notificationService = new NotificationService();
            orderServices = new OrderServices();
        }

        [HttpGet]
        public ActionResult WarrantyDetails()
        {
            return View();
        }

        [HttpPost]
        public ActionResult SearchWarranty(string warrantyCode)
        {
            var warranty = _warrantyService.GetWarrantyByCode(warrantyCode);
            if (warranty == null)
            {
                ViewBag.ErrorMessage = "Warranty not found.";
                return View("WarrantyDetails");
            }

            return View("WarrantyDetails", warranty);
        }

        [HttpPost]
        public ActionResult SendWarranty(WarrantyDetailsViewModel model)
        {
            if (ModelState.IsValid)
            {
                _warrantyService.UpdateWarrantyDetails(model.WarrantyCode, model.WarrantyDetails);

                // Get the logged-in user's ID from the session
                var userID = Session["UserID"]?.ToString();
                if (string.IsNullOrEmpty(userID))
                {
                    ViewBag.ErrorMessage = "User is not logged in.";
                    return View("WarrantyDetails", model);
                }

                _notificationService.AddNotification(new tblNotification
                {
                    userID = userID, // Use the logged-in user's ID
                    date = DateTime.Now,
                    detail = "Your warranty request has been submitted.",
                    status = true
                });

                ViewBag.SuccessMessage = "Warranty details updated successfully.";
            }
            else
            {
                ViewBag.ErrorMessage = "Please provide valid details.";
            }

            var warranty = _warrantyService.GetWarrantyByCode(model.WarrantyCode);
            return View("WarrantyDetails", warranty);
        }


        public ActionResult ViewWarranty()
        {
            var userID = Session["UserID"]?.ToString();
            if (string.IsNullOrEmpty(userID))
            {
                return RedirectToAction("Index", "Account");
            }

            var warranties = _warrantyService.GetNonValidWarrantiesByCustomer(userID);

            if (warranties == null || !warranties.Any())
            {
                System.Diagnostics.Debug.WriteLine($"No warranties found for customer {userID} with non-valid status.");
            }
            else
            {
                System.Diagnostics.Debug.WriteLine($"Found {warranties.Count} warranties for customer {userID} with non-valid status.");
            }
            return View("CustomerWarranties", warranties);
        }

        public ActionResult ViewWarrantyDetail(int warrantyID)
        {
            var warranty = _warrantyService.GetWarrantyByID(warrantyID);
            if (warranty == null)
            {
                ViewBag.ErrorMessage = "Warranty not found.";
                return View("ViewWarranty");
            }

            return View("ViewWarranty", warranty);
        }

        [HttpPost]
        public ActionResult SubmitWarranty(tblWarranty warranty)
        {
            _warrantyService.SubmitWarranty(warranty);

            var order = orderServices.GetOrderById(warranty.orderID);
            var customerId = order.customerID;

            _notificationService.AddNotification(new tblNotification
            {
                userID = customerId,
                date = DateTime.Now,
                detail = "Your warranty request has been submitted.",
                status = true
            });

            return RedirectToAction("Index");
        }

        [HttpPost]
        public ActionResult ProcessWarranty(int warrantyID)
        {
            try
            {
                var result = _warrantyService.ProcessWarranty(warrantyID);
                if (result)
                {
                    return Json(new { success = true });
                }
                else
                {
                    return Json(new { success = false, message = "Unable to process warranty." });
                }
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }



    }
}
