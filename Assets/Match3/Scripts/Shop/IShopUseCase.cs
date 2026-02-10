using Match3.Scripts.Shop.Item;

namespace Match3.Scripts.Shop
{
    public interface IShopUseCase
    {
        bool BuyPowerUp(ShopItemModel item);
        void GiveStats(ShopItemModel item);
    }
}