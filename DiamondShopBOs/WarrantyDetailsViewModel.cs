using System;

namespace DiamondShopBOs
{
    public class WarrantyDetailsViewModel
    {
        public string WarrantyCode { get; set; }
        public DateTime WarrantyStartDate { get; set; }
        public DateTime WarrantyEndDate { get; set; }
        public string WarrantyDetails { get; set; }

        // Properties for displaying item details
        public string DiamondName { get; set; }
        public decimal DiamondPrice { get; set; }
        public string ImagePath { get; set; }
        public string SettingName { get; set; }
        public decimal SettingPrice { get; set; }
        public string AccentStoneName { get; set; }
        public decimal AccentStonePrice { get; set; }
        public int AccentStoneQuantity { get; set; }

        public string FullName { get; set; }
        public string CustomerName { get; set; }
        public string OrderID { get; set; }
        public string UserID { get; set; }
    }
}
