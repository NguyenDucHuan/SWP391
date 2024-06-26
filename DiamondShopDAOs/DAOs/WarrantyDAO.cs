using DiamondShopBOs;
using System;
using System.Collections.Generic;
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

        public void CreateWarranty(string orderId, int itemId, string warrantyCode, string warrantyDetails)
        {
            var warranty = new tblWarranty
            {
                orderID = orderId,
                ItemID = itemId,
                warrantyCode = warrantyCode,
                warrantyStartDate = DateTime.Now,
                warrantyEndDate = DateTime.Now.AddMonths(3),
                warrantyDetails = "No Detail",
                status = "Valid" // Đặt trạng thái ban đầu là "No Process"
            };

            _context.tblWarranties.Add(warranty);
            _context.SaveChanges();
        }

        public tblWarranty GetWarrantyByCode(string warrantyCode)
        {
            return _context.tblWarranties.FirstOrDefault(w => w.warrantyCode == warrantyCode);
        }
        public tblWarranty GetWarrantyByID(int warrantyID)
        {
            return _context.tblWarranties.FirstOrDefault(w => w.warrantyID == warrantyID);
        }

        public void UpdateWarrantyDetails(string warrantyCode, string newDetails)
        {
            var existingWarranty = _context.tblWarranties.FirstOrDefault(w => w.warrantyCode == warrantyCode);

            if (existingWarranty != null)
            {
                // Tạo mới warranty với nội dung giống cái cũ và chỉ thay đổi warrantyDetails
                var newWarranty = new tblWarranty
                {
                    orderID = existingWarranty.orderID,
                    ItemID = existingWarranty.ItemID,
                    warrantyCode = existingWarranty.warrantyCode,
                    warrantyStartDate = existingWarranty.warrantyStartDate,
                    warrantyEndDate = existingWarranty.warrantyEndDate,
                    warrantyDetails = newDetails,
                    status = "No Process"
                };

                _context.tblWarranties.Add(newWarranty);
                _context.SaveChanges();
            }
        }
        public List<tblWarranty> GetNonValidWarrantiesByCustomer(string userID)
        {
            var warranties = _context.tblWarranties
                                    .Where(w => w.tblOrderItem.tblOrder.customerID == userID && w.status != "Valid")
                                    .ToList();
            System.Diagnostics.Debug.WriteLine($"Found {warranties.Count} warranties for customer {userID} with non-valid status.");
            return warranties;
        }

        public tblWarranty GetWarrantyById(int warrantyId)
        {
            return _context.tblWarranties.Find(warrantyId);
        }

        public void ProcessWarranty(tblWarranty warranty)
        {
            warranty.status = "Processed";
            _context.SaveChanges();
        }

        public void SubmitWarranty(tblWarranty warranty)
        {
            _context.tblWarranties.Add(warranty);
            _context.SaveChanges();
        }

    }
}
