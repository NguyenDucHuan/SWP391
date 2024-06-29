using DiamondShopBOs;
using DiamondShopRepositories.WarrantyRepository;
using DiamondShopServices.NotificationService;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Remoting.Contexts;

namespace DiamondShopServices.WarrantyServices
{
    public class WarrantyService : IWarrantyService
    {
        private readonly IWarrantyRepository _warrantyRepository;
        private readonly INotificationService _notificationService;

        public WarrantyService()
        {
            _warrantyRepository = new WarrantyRepository();
            _notificationService = new DiamondShopServices.NotificationService.NotificationService();
        }

        public WarrantyDetailsViewModel GetWarrantyByCode(string warrantyCode)
        {
            var warranty = _warrantyRepository.GetWarrantyByCode(warrantyCode);
            if (warranty == null) return null;

            var item = warranty.tblOrderItem.tblItem;

            var viewModel = new WarrantyDetailsViewModel
            {
                WarrantyCode = warranty.warrantyCode,
                WarrantyStartDate = warranty.warrantyStartDate,
                WarrantyEndDate = warranty.warrantyEndDate,
                WarrantyDetails = warranty.warrantyDetails,
                DiamondName = item.tblDiamond?.diamondName,
                DiamondPrice = item.tblDiamond?.diamondPrice ?? 0,
                ImagePath = item.tblDiamond?.diamondImagePath,
                SettingName = item.tblSetting?.settingType,
                SettingPrice = item.tblSetting?.priceTax ?? 0,
                AccentStoneName = item.tblAccentStone?.accentStonesName,
                AccentStonePrice = item.tblAccentStone?.price ?? 0,
                AccentStoneQuantity = item.quantityAccent ?? 0,
                FullName = warranty.tblOrderItem.tblOrder.tblUser.fullName,
                CustomerName = warranty.tblOrderItem.tblOrder.customerName
            };

            return viewModel;
        }

        public void UpdateWarrantyDetails(string warrantyCode, string warrantyDetails)
        {
            _warrantyRepository.UpdateWarrantyDetails(warrantyCode, warrantyDetails);
        }

        public void CreateWarranty(tblWarranty warranty)
        {
            _warrantyRepository.CreateWarranty(warranty);
        }
        public List<tblWarranty> GetNonValidWarrantiesByCustomer(string customerId)
        {
            return _warrantyRepository.GetNonValidWarrantiesByCustomer(customerId);
        }
        public WarrantyDetailsViewModel GetWarrantyByID(int warrantyID)
        {
            var warranty = _warrantyRepository.GetWarrantyByID(warrantyID);
            if (warranty == null) return null;

            var item = warranty.tblOrderItem.tblItem;

            var viewModel = new WarrantyDetailsViewModel
            {
                WarrantyCode = warranty.warrantyCode,
                WarrantyStartDate = warranty.warrantyStartDate,
                WarrantyEndDate = warranty.warrantyEndDate,
                WarrantyDetails = warranty.warrantyDetails,
                DiamondName = item.tblDiamond?.diamondName,
                DiamondPrice = item.tblDiamond?.diamondPrice ?? 0,
                ImagePath = item.tblDiamond?.diamondImagePath,
                SettingName = item.tblSetting?.settingType,
                SettingPrice = item.tblSetting?.priceTax ?? 0,
                AccentStoneName = item.tblAccentStone?.accentStonesName,
                AccentStonePrice = item.tblAccentStone?.price ?? 0,
                AccentStoneQuantity = item.quantityAccent ?? 0,
                FullName = warranty.tblOrderItem.tblOrder.tblUser.fullName,
                CustomerName = warranty.tblOrderItem.tblOrder.customerName
            };

            return viewModel;
        }
        public bool ProcessWarranty(int warrantyID)
        {
            var warranty = _warrantyRepository.GetWarrantyByID(warrantyID);
            if (warranty != null && warranty.status != "Processed")
            {
                warranty.status = "Processed";
                _warrantyRepository.UpdateWarranty(warranty);

                // Create notification if needed
                var notification = new tblNotification
                {
                    userID = warranty.tblOrderItem.tblOrder.customerID,
                    date = DateTime.Now,
                    detail = $"Your warranty (ID: {warrantyID}) has been processed.",
                    status = true
                };
                _notificationService.AddNotification(notification);

                return true;
            }
            return false;
        }



        public void SubmitWarranty(tblWarranty warranty)
        {
            _warrantyRepository.SubmitWarranty(warranty);
        }
        public List<tblWarranty> GetNonValidWarranties()
        {
            return _warrantyRepository.GetNonValidWarranties();
        }

    }
}
