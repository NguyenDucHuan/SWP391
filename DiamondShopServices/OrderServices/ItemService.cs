using DiamondShopRepositories.OrderRepositories;

namespace DiamondShopServices.OrderServices
{
    public class ItemService : IItemService
    {
        private readonly IItemRepository repository;

        public ItemService()
        {
            repository = new ItemRepository();
        }

        public void CreateItem(string orderID, int? settingID, int? accentStoneID, int? quantityAccent, int diamondID, decimal diamondPrice, decimal settingPrice, decimal accentPrice)
        {
            repository.CreateItem(orderID, settingID, accentStoneID, quantityAccent, diamondID, diamondPrice, settingPrice, accentPrice);
        }
    }
}
