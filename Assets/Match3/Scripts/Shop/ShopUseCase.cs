using Match3.Scripts.Persistence;
using Match3.Scripts.Player;
using Match3.Scripts.Shop.Item;
using Match3.Scripts.Utils;

namespace Match3.Scripts.Shop
{
    public class ShopUseCase : IShopUseCase
    {
        private readonly ShopItemModel _shopItem;
        private readonly PlayerModel _player;
        private readonly IProgressRepository _progressRepository;

        public ShopUseCase(PlayerModel player)
        {
            _player = player;
            _progressRepository = ServiceLocator.ServiceLocator.Global.Get<IProgressRepository>();
        }

        public bool BuyPowerUp(ShopItemModel item)
        {
            if (!_player.CanAfford(item.Price))
                return false;

            _player.SpendCoins(item.Price);

            if (!string.IsNullOrEmpty(item.PowerUpId))
            {
                _progressRepository.AddPowerUp(item.PowerUpId, 1);
            }

            _progressRepository.SavePlayerData(_player.Coins, _player.Health);
            EventDispatcherService.Instance.Dispatch(_player);
            return true;
        }
        public void GiveStats(ShopItemModel item)
        {
            if (item.RewardType == ShopRewardType.Coins)
                _player.AddCoins(item.RewardAmount);

            if (item.RewardType == ShopRewardType.Health)
                _player.AddHealth(item.RewardAmount);

            _progressRepository.SavePlayerData(_player.Coins, _player.Health);
            EventDispatcherService.Instance.Dispatch(_player);
        }
    }
}