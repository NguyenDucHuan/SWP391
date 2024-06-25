using DiamondShopServices;
using DiamondShopBOs;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web.Mvc;
using Newtonsoft.Json.Linq;
using DiamondShopServices.JewelrySettingService;
using DiamondShopServices.AccentStoneService;
using ProjectDiamondShop.Models;
using System.IO;
using System.Web;

namespace ProjectDiamondShop.Controllers
{
    public class DiamondsController : Controller
    {
        private readonly IDiamondService diamondService = null;
        private readonly IAccentStoneService accentStoneService = null;
        private readonly IJewelrySettingService jewelrySettingService = null;
        private bool IsAdmin()
        {
            return Session["RoleID"] != null && (int)Session["RoleID"] == 2;
        }
        private bool IsSaleStaff()
        {
            return Session["RoleID"] != null && (int)Session["RoleID"] == 5;
        }
        private bool IsDelivery()
        {
            return Session["RoleID"] != null && (int)Session["RoleID"] == 4;
        }
        private bool IsManager()
        {
            return Session["RoleID"] != null && (int)Session["RoleID"] == 3;
        }
        public DiamondsController()
        {
            if (diamondService == null)
            {
                diamondService = new DiamondService();
            }
            if (jewelrySettingService == null)
            {
                jewelrySettingService = new JewelrySettingService();
            }
            if (accentStoneService == null)
            {
                accentStoneService = new AccentStoneService();
            }
        }

        public ActionResult Index(int page = 1, int pageSize = 12)
        {
            if (IsAdmin())
            {
                return RedirectToAction("Index", "Manager");
            }
            if (IsManager())
            {
                return RedirectToAction("Index", "Manager");
            }
            if (IsSaleStaff())
            {
                return RedirectToAction("Index", "SaleStaff");
            }
            if (IsDelivery())
            {
                return RedirectToAction("Index", "DeliveryStaff");
            }
            ViewBag.minPrice = 1;
            ViewBag.maxPrice = 99999;
            ViewBag.minCaratWeight = 0.5;
            ViewBag.maxCaratWeight = 10;
            ViewBag.CurrentPage = page;
            ViewBag.PageSize = pageSize;
            ViewBag.SelectedShape = "";
            ViewBag.SelectedColor = "";
            ViewBag.SelectedCut = "";
            ViewBag.SelectedClarity = "";
            ViewBag.SelectedSort = "Price (Low to High)";

            List<tblDiamond> diamonds = diamondService.Filter("", "", "", "", "", null, null, null, null, "Price (Low to High)");

            int totalDiamonds = diamonds.Count;
            ViewBag.NumOfPage = (int)Math.Ceiling((double)totalDiamonds / pageSize);

            List<tblDiamond> diamondsForPage = diamonds.Skip((page - 1) * pageSize).Take(pageSize).ToList();

            return View("Diamonds", diamondsForPage);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Filter(int page = 1, int pageSize = 12)
        {
            string searchTerm = Request.Form["SearchTerm"];
            if (!String.IsNullOrEmpty(searchTerm))
            {
                searchTerm = searchTerm.Trim();
            }
            string clarity = Request.Form["Clarity"];
            string cut = Request.Form["Cut"];
            string color = Request.Form["Color"];
            string shape = Request.Form["Shape"];
            string sortBy = Request.Form["sortBy"];
            decimal? minPrice = null;
            decimal? maxPrice = null;
            float? minCaratWeight = null;
            float? maxCaratWeight = null;

            if (decimal.TryParse(Request.Form["MinPrice"], out decimal minPriceValue))
            {
                minPrice = minPriceValue;
                ViewBag.MinPrice = minPrice;
            }
            if (decimal.TryParse(Request.Form["MaxPrice"], out decimal maxPriceValue))
            {
                maxPrice = maxPriceValue;
                ViewBag.MaxPrice = maxPrice;
            }
            if (float.TryParse(Request.Form["MinCaratWeight"], out float minCaratWeightValue))
            {
                minCaratWeight = minCaratWeightValue;
                ViewBag.MinCaratWeight = minCaratWeight;
            }
            if (float.TryParse(Request.Form["MaxCaratWeight"], out float maxCaratWeightValue))
            {
                maxCaratWeight = maxCaratWeightValue;
                ViewBag.MaxCaratWeight = maxCaratWeight;
            }

            ViewBag.SelectedShape = shape;
            ViewBag.SelectedColor = color;
            ViewBag.SelectedCut = cut;
            ViewBag.SelectedClarity = clarity;
            ViewBag.SelectedSort = sortBy;

            List<tblDiamond> diamonds = diamondService.Filter(searchTerm, clarity, cut, color, shape, minPrice, maxPrice, minCaratWeight, maxCaratWeight, sortBy);

            int totalDiamonds = diamonds.Count;
            ViewBag.NumOfPage = (int)Math.Ceiling((double)totalDiamonds / pageSize);
            ViewBag.CurrentPage = page;

            List<tblDiamond> diamondsForPage = diamonds.Skip((page - 1) * pageSize).Take(pageSize).ToList();

            return View("Diamonds", diamondsForPage);
        }


        public ActionResult ViewDiamond(int id)
        {
            if (IsAdmin())
            {
                return RedirectToAction("Index", "Manager");
            }
            if (IsManager())
            {
                return RedirectToAction("Index", "Manager");
            }
            if (IsSaleStaff())
            {
                return RedirectToAction("Index", "SaleStaff");
            }
            if (IsDelivery())
            {
                return RedirectToAction("Index", "DeliveryStaff");
            }
            tblDiamond diamond = diamondService.GetDiamondById(id);
            List<tblAccentStone> accentStones = accentStoneService.GetAllStones();
            List<tblSetting> settings = jewelrySettingService.GetSettingAllList();
            List<tblCertificate> certificates =
                 diamondService.GetCertificatesByDiamondId(id);

            ViewBag.AccentStones = accentStones;
            ViewBag.Settings = settings;
            ViewBag.Certificates = certificates;
            return View(diamond);
        }

        [HttpGet]
        public JsonResult GetSettings()
        {
            var settings = jewelrySettingService.GetSettingAllList();
            var settingsData = settings.Select(s => new
            {
                s.settingID,
                s.settingType,
                s.material,
                s.priceTax,
                s.quantityStones,
                s.description,
                s.imagePath
            }).ToList();

            return Json(settingsData, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult GetAccentStones()
        {
            var accentStones = accentStoneService.GetAllStones();
            var accentStonesData = accentStones.Select(a => new
            {
                a.accentStoneID,
                a.shape,
                a.caratWeight,
                a.clarity,
                a.color,
                a.price,
                a.quantity,
                a.imagePath
            }).ToList();

            return Json(accentStonesData, JsonRequestBehavior.AllowGet);
        }

        public ActionResult AddDiamond()
        {
            if (Session["RoleID"] == null || (int)Session["RoleID"] != 3 && (int)Session["RoleID"] != 2)
            {
                return RedirectToAction("Index", "Home");
            }
            return View();
        }

        [HttpPost]
        public ActionResult AddDiamond(tblDiamond model, HttpPostedFileBase diamondImageA, HttpPostedFileBase diamondImageB, HttpPostedFileBase diamondImageC, HttpPostedFileBase certificateImage, string certificateNumber, DateTime? issueDate, string certifyingAuthority)
        {
            if (Session["RoleID"] == null || (int)Session["RoleID"] != 3 && (int)Session["RoleID"] != 2)
            {
                return RedirectToAction("Index", "Home");
            }

            if (ModelState.IsValid)
            {
                string diamondImagePaths = "";
                string certificateImagePath = "";

                // Get the next available diamond image number
                int nextImageNumber = GetNextDiamondImageNumber();

                // Save diamond images
                if (diamondImageA != null && diamondImageA.ContentLength > 0)
                {
                    string fileName = $"dia{nextImageNumber}A.png";
                    string path = Path.Combine(Server.MapPath("~/Image/DiamondDTO/Diamonds"), fileName);
                    diamondImageA.SaveAs(path);
                    diamondImagePaths += $"/Image/DiamondDTO/Diamonds/{fileName}|";
                }

                if (diamondImageB != null && diamondImageB.ContentLength > 0)
                {
                    string fileName = $"dia{nextImageNumber}B.png";
                    string path = Path.Combine(Server.MapPath("~/Image/DiamondDTO/Diamonds"), fileName);
                    diamondImageB.SaveAs(path);
                    diamondImagePaths += $"/Image/DiamondDTO/Diamonds/{fileName}|";
                }

                if (diamondImageC != null && diamondImageC.ContentLength > 0)
                {
                    string fileName = $"dia{nextImageNumber}C.png";
                    string path = Path.Combine(Server.MapPath("~/Image/DiamondDTO/Diamonds"), fileName);
                    diamondImageC.SaveAs(path);
                    diamondImagePaths += $"/Image/DiamondDTO/Diamonds/{fileName}|";
                }

                diamondImagePaths = diamondImagePaths.TrimEnd('|');

                // Save certificate image
                if (certificateImage != null && certificateImage.ContentLength > 0)
                {
                    string fileName = $"CER{nextImageNumber:D2}.jpg";
                    string path = Path.Combine(Server.MapPath("~/Image/DiamondDTO/Certificates"), fileName);
                    certificateImage.SaveAs(path);
                    certificateImagePath = $"/Image/DiamondDTO/Certificates/{fileName}";
                }

                // Create Diamond and Certificate objects
                tblDiamond newDiamond = new tblDiamond
                {
                    diamondName = model.diamondName,
                    diamondPrice = model.diamondPrice,
                    diamondDescription = model.diamondDescription,
                    caratWeight = Math.Round(model.caratWeight, 2),
                    clarityID = model.clarityID,
                    cutID = model.cutID,
                    colorID = model.colorID,
                    shapeID = model.shapeID,
                    diamondImagePath = diamondImagePaths,
                    status = true,
                    quantity = 1
                };

                tblCertificate newCertificate = new tblCertificate
                {
                    certificateNumber = certificateNumber,
                    issueDate = issueDate ?? DateTime.Now,
                    certifyingAuthority = certifyingAuthority,
                    cerImagePath = certificateImagePath
                };

                diamondService.AddNewDiamond(newDiamond, newCertificate);

                TempData["SuccessMessage"] = "Diamond added successfully!";
                return RedirectToAction("AddDiamond");
            }

            return View(model);
        }

        private int GetNextDiamondImageNumber()
        {
            using (var context = new DiamondShopManagementEntities())
            {
                return (context.tblDiamonds.Max(d => (int?)d.diamondID) ?? 0) + 1;
            }


        }
    }
}
