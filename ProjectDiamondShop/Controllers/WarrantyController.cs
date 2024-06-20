using System.Linq;
using System.Web.Mvc;
using DiamondShopBOs;
using DiamondShopServices.WarrantyServices;

namespace ProjectDiamondShop.Controllers
{
    public class WarrantyController : Controller
    {
        private readonly IWarrantyService _warrantyService;

        public WarrantyController()
        {
            _warrantyService = new WarrantyService();
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
                return RedirectToAction("Index", "Login");
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



    }
}
