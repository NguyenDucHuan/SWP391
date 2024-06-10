using DiamondShopBOs;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiamondShopDAOs
{
    public class DiamondDAO
    {
        private readonly DiamondShopManagementEntities diamondShopManagementEntities;
        public DiamondDAO()
        {
            diamondShopManagementEntities = new DiamondShopManagementEntities();
        }
        public List<tblDiamond> GetAllDiamonds()
        {
            var diamonds = diamondShopManagementEntities.tblDiamonds.ToList();
            return diamonds;
        }
        public tblDiamond GetDiamondById(int id)
        {
            var diamond = diamondShopManagementEntities.tblDiamonds.FirstOrDefault(d => d.diamondID == id);
            return diamond;
        }
        public tblDiamond UpdateDiamond(int id, tblDiamond diamond)
        {
            var Entity = new DiamondShopManagementEntities();
            var inDiamond = Entity.tblDiamonds.Where(d => d.diamondID == id).FirstOrDefault();
            if (inDiamond != null)
            {
                inDiamond.diamondID = diamond.diamondID;
                inDiamond.diamondName = diamond.diamondName;
                inDiamond.diamondPrice = diamond.diamondPrice;
                inDiamond.diamondDescription = diamond.diamondDescription;
                inDiamond.diamondImagePath = diamond.diamondImagePath;
                inDiamond.caratWeight = diamond.caratWeight;
                inDiamond.cutID = diamond.cutID;
                inDiamond.clarityID = diamond.clarityID;
                inDiamond.colorID = diamond.colorID;

            }
            Entity.SaveChanges();
            return inDiamond;
        }
        public void AddNewDiamond(tblDiamond newDiamond)
        {
            using (var diamondShopManagementEntities = new DiamondShopManagementEntities())
            {
                diamondShopManagementEntities.tblDiamonds.Add(newDiamond);
                diamondShopManagementEntities.SaveChanges();
            }
        }
        public List<tblDiamond> Filter(string searchTerm, string clarity, string cut, string color, string shape, decimal? minPrice, decimal? maxPrice, float? minCaratWeight, float? maxCaratWeight, string sortBy)
        {
            var query = diamondShopManagementEntities.tblDiamonds.AsQueryable();

            if (!String.IsNullOrEmpty(searchTerm))
            {
                query = query.Where(d => d.diamondName.Contains(searchTerm));
            }
            if (!String.IsNullOrEmpty(clarity))
            {
                query = query.Where(d => d.clarityID.Contains(clarity));
            }
            if (!String.IsNullOrEmpty(cut))
            {
                query = query.Where(d => d.cutID.Contains(cut));
            }
            if (!String.IsNullOrEmpty(color))
            {
                query = query.Where(d => d.colorID.Contains(color));
            }
            if (!String.IsNullOrEmpty(shape))
            {
                query = query.Where(d => d.shapeID.Contains(shape));
            }
            if (minPrice.HasValue)
            {
                query = query.Where(d => d.diamondPrice >= minPrice.Value);
            }
            if (maxPrice.HasValue)
            {
                query = query.Where(d => d.diamondPrice <= maxPrice.Value);
            }
            if (minCaratWeight.HasValue)
            {
                query = query.Where(d => d.caratWeight >= minCaratWeight.Value);
            }
            if (maxCaratWeight.HasValue)
            {
                query = query.Where(d => d.caratWeight <= maxCaratWeight.Value);
            }
            switch (sortBy)
            {
                case "Price (Low to High)":
                    query = query.OrderBy(d => d.diamondPrice);
                    break;
                case "Price (High to Low)":
                    query = query.OrderByDescending(d => d.diamondPrice);
                    break;
                case "Carat Weight (Low to High)":
                    query = query.OrderBy(d => d.caratWeight);
                    break;
                case "Carat Weight (High to Low)":
                    query = query.OrderByDescending(d => d.caratWeight);
                    break;
                default:
                    query = query.OrderBy(d => d.diamondID); // Default sorting by ID
                    break;
            }
            return query.ToList();
        }
    }
}
