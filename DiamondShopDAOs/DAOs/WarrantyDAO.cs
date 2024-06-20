using DiamondShopBOs;
using System;
using System.Linq;

namespace DiamondShopDAOs
{
    public class WarrantyDAO
    {
        private readonly DiamondShopManagementEntities _context;

        public WarrantyDAO()
        {
            _context = new DiamondShopManagementEntities();
        }

        public string GenerateWarrantyCode()
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            var random = new Random();
            return new string(Enumerable.Repeat(chars, 6).Select(s => s[random.Next(s.Length)]).ToArray());
        }

        public void CreateWarranty(string orderId, int itemId, string warrantyCode)
        {
            var warranty = new tblWarranty
            {
                orderID = orderId,
                ItemID = itemId,
                warrantyCode = warrantyCode,
                warrantyStartDate = DateTime.Now,
                warrantyEndDate = DateTime.Now.AddMonths(3),
                warrantyDetails = "No Detail"
            };

            _context.tblWarranties.Add(warranty);
            _context.SaveChanges();
        }

        public tblWarranty GetWarrantyByCode(string warrantyCode)
        {
            return _context.tblWarranties.FirstOrDefault(w => w.warrantyCode == warrantyCode);
        }

        public void UpdateWarrantyDetails(string warrantyCode, string details)
        {
            var warranty = _context.tblWarranties.FirstOrDefault(w => w.warrantyCode == warrantyCode);
            if (warranty != null)
            {
                warranty.warrantyDetails = details;
                _context.SaveChanges();
            }
        }
    }
}
