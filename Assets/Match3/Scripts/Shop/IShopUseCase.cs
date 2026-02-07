using Match3.Scripts.Shop.Item;

namespace Match3.Scripts.Shop
{
    public interface IShopUseCase
    {
        bool Execute(ShopItemModel item);
        void ExecuteReward(ShopItemModel item);
    }
}