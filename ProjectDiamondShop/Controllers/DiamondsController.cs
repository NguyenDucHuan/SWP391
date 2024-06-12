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

namespace ProjectDiamondShop.Controllers
{
    public class DiamondsController : Controller
    {
        // GET: Diamonds
        private readonly IDiamondService diamondService = null;
        private readonly IAccentStoneService accentStoneService = null;
        private readonly IJewelrySettingService jewelrySettingService = null;
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
            ViewBag.SelectedSort = "";

            List<tblDiamond> diamonds = diamondService.Filter("", "", "", "", "", null, null, null, null, "Price (Low to High)");

            int totalDiamonds = diamonds.Count;
            ViewBag.NumOfPage = (int)Math.Ceiling((double)totalDiamonds / pageSize);

            List<tblDiamond> diamondsForPage = diamonds.Skip((page - 1) * pageSize).Take(pageSize).ToList();

            return View("Diamonds", diamondsForPage);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Fillter(string sortBy, int page = 1, int pageSize = 12)
        {
            string searchTerm = Request.Form["SearchTerm"];
            string clarity = Request.Form["Clarity"];
            string cut = Request.Form["Cut"];
            string color = Request.Form["Color"];
            string shape = Request.Form["Shape"];
            decimal? minPrice = null;
            decimal? maxPrice = null;
            float? minCaratWeight = null;
            float? maxCaratWeight = null;

            if (decimal.TryParse(Request.Form["MinPrice"], out decimal minPriceValue))
            {
                minPrice = minPriceValue;
                ViewBag.minPrice = minPrice;
            }
            if (decimal.TryParse(Request.Form["MaxPrice"], out decimal maxPriceValue))
            {
                maxPrice = maxPriceValue;
                ViewBag.maxPrice = maxPrice;
            }
            if (float.TryParse(Request.Form["MinCaratWeight"], out float minCaratWeightValue))
            {
                minCaratWeight = minCaratWeightValue;
                ViewBag.minCaratWeight = minCaratWeight;
            }
            if (float.TryParse(Request.Form["MaxCaratWeight"], out float maxCaratWeightValue))
            {
                maxCaratWeight = maxCaratWeightValue;
                ViewBag.maxCaratWeight = maxCaratWeight;
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
            tblDiamond diamond = diamondService.GetDiamondById(id);
            List<tblAccentStone> accentStones = accentStoneService.GetAllStones();
            List<tblSetting> settings = jewelrySettingService.GetSettingAllList();

            ViewBag.AccentStones = accentStones;
            ViewBag.Settings = settings;

            return View(diamond);
        }

    }

}