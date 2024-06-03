﻿using ProjectDiamondShop.Models;
using ProjectDiamondShop.Repositories;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web.Mvc;

namespace ProjectDiamondShop.Controllers
{
    public class DiamondsController : Controller
    {
        // GET: Diamonds
        private static readonly string connectionString = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;
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

            DiamondRepository diamondRepository = new DiamondRepository(connectionString);
            List<Diamond> diamonds = diamondRepository.GetFilteredDiamonds("", "", "", "", "", null, null, null, null, null);

            int totalDiamonds = diamonds.Count;
            ViewBag.NumOfPage = (int)Math.Ceiling((double)totalDiamonds / pageSize);

            List<Diamond> diamondsForPage = diamonds.Skip((page - 1) * pageSize).Take(pageSize).ToList();

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

            DiamondRepository diamondRepository = new DiamondRepository(connectionString);
            List<Diamond> filteredDiamonds = diamondRepository.GetFilteredDiamonds(
                searchTerm, clarity, cut, color, shape, minPrice, maxPrice, minCaratWeight, maxCaratWeight, sortBy);

            int totalDiamonds = filteredDiamonds.Count;
            ViewBag.NumOfPage = (int)Math.Ceiling((double)totalDiamonds / pageSize);
            ViewBag.CurrentPage = page;

            List<Diamond> diamondsForPage = filteredDiamonds.Skip((page - 1) * pageSize).Take(pageSize).ToList();

            return View("Diamonds", diamondsForPage);
        }

        public ActionResult ViewDiamond(int id)
        {
            DiamondRepository diamondRepository = new DiamondRepository(connectionString);
            Diamond diamond = diamondRepository.GetDiamond(id);
            return View(diamond);
        }
    }

}