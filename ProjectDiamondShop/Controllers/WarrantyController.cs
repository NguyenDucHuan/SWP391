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
    }
}
