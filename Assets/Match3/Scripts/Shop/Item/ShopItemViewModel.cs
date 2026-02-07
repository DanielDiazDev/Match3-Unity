using UniRx;
using UnityEngine;

namespace Match3.Scripts.Shop.Item
{
    public class ShopItemViewModel
    {
        public string Id { get; }
        public ShopCategoryType Category { get; }

        public ReactiveProperty<string> Name;
        public ReactiveProperty<string> PriceText;
        public ReactiveProperty<Sprite> Icon;

        public ReactiveCommand Clicked;

        public ShopItemViewModel(ShopItemModel item, Sprite icon)
        {
            Id = item.Id;
            Category = item.Category;

            Name = new ReactiveProperty<string>(item.Name);
            Icon = new ReactiveProperty<Sprite>(icon);

            if (item.Category == ShopCategoryType.PowerUps)
                PriceText = new ReactiveProperty<string>($"{item.Price} Coins");
            else
                PriceText = new ReactiveProperty<string>("Watch Ad");

            Clicked = new ReactiveCommand();
        }
    }

}