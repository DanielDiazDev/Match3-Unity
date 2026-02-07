using Match3.Scripts.Shop.Item;
using UniRx;

namespace Match3.Scripts.Shop
{
    public class ShopViewModel
    {
        public ReactiveProperty<ShopCategoryType> SelectedCategory;
        public ReactiveCollection<ShopItemViewModel> VisibleItems;

        public ReactiveCommand SelectPowerUpsTab;
        public ReactiveCommand SelectStatsTab;

        public ShopViewModel()
        {
            SelectedCategory = new ReactiveProperty<ShopCategoryType>(ShopCategoryType.PowerUps);
            VisibleItems = new ReactiveCollection<ShopItemViewModel>();

            SelectPowerUpsTab = new ReactiveCommand();
            SelectStatsTab = new ReactiveCommand();
        }
    }
}