using ProjectDiamondShop.Models;
using ProjectDiamondShop.Repositories;
using System.Collections.Generic;
using System.Configuration;
using System.Web.Mvc;

namespace ProjectDiamondShop.Controllers
{
    public class DiamondsController : Controller
    {
        // GET: Diamonds
        private static readonly string connectionString = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;
        public ActionResult Index()
        {
            ViewBag.minPrice = 1;
            ViewBag.maxPrice = 99999;
            ViewBag.minCaratWeight = 0.5;
            ViewBag.maxCaratWeight = 5;
            DiamondRepository diamondRepository = new DiamondRepository(connectionString);
            List<Diamond> diamonds = diamondRepository.GetFilteredDiamonds("","","","","",null,null,null,null);
            return View("Diamonds", diamonds);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Fillter()
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
            ViewBag.SelectedShape =shape;
            ViewBag.SelectedColor =color;
            ViewBag.SelectedCut =cut;
            ViewBag.SelectedClarity =clarity;
            
            DiamondRepository diamondRepository = new DiamondRepository(connectionString);
            List<Diamond> filteredDiamonds = diamondRepository.GetFilteredDiamonds(
                searchTerm, clarity, cut, color, shape, minPrice, maxPrice, minCaratWeight, maxCaratWeight);
            return View("Diamonds", filteredDiamonds);
        }
    }

}