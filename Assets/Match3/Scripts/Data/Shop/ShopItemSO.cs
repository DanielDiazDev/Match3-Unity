using Match3.Scripts.Data.Item;
using Match3.Scripts.Shop;
using UnityEngine;
using ShopCategoryType = Match3.Scripts.Shop.Item.ShopCategoryType;

namespace Match3.Scripts.Data.Shop
{
    [CreateAssetMenu(fileName = "ShopItemSO", menuName = "Match3/Shop/ShopItemSO")]
    public class ShopItemSO : ScriptableObject
    {
        public string id;
        public string productName;
        public Sprite icon;

        public ShopCategoryType category;

        [Header("PowerUp")]
        public PowerUpSO powerUp;
        public int price;

        [Header("Reward (Stats Category)")]
        public ShopRewardType rewardType;
        public int rewardAmount;
    }
}
