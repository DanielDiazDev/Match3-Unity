using Match3.Scripts.Data.Shop;
using UnityEngine;

namespace Match3.Scripts.Shop.Item
{
    public class ShopItemModel
    {
        public string Id { get; }
        public string Name { get; }
        public int Price { get; }
        public ShopCategoryType Category { get; }
        public ShopRewardType RewardType { get; }
        public int RewardAmount { get; }
        public string PowerUpId { get; }
        public Sprite Icon { get; }

        public ShopItemModel(ShopItemSO so)
        {
            Id = so.id;
            Name = so.productName;
            Price = so.price;
            Category = so.category;
            RewardType = so.rewardType;
            RewardAmount = so.rewardAmount;
            PowerUpId = so.powerUp != null ? so.powerUp.id : string.Empty;
            Icon = so.icon;
        }
    }
    public enum ShopCategoryType
    {
        PowerUps,
        Stats
    }
}