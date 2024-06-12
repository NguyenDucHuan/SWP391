namespace DiamondShopServices.OrderServices
{
    public interface IItemService
    {
        void CreateItem(string orderID, int? settingID, int? accentStoneID, int? quantityAccent, int diamondID, decimal diamondPrice, decimal settingPrice, decimal accentPrice);
    }
}
