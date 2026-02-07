using System;
using Match3.Scripts.Utils;

namespace Match3.Scripts.Player
{
    public class PlayerPresenter : IDisposable
    {
        private readonly PlayerViewModel _model;

        public PlayerPresenter(PlayerViewModel model)
        {
            _model = model;

            EventDispatcherService.Instance.Subscribe<PlayerModel>(OnPlayerUpdated);
        }

        private void OnPlayerUpdated(PlayerModel player)
        {
            _model.Coins.Value = player.Coins;
            _model.Health.Value = player.Health;
        }

        public void Dispose()
        {
            EventDispatcherService.Instance.Unsubscribe<PlayerModel>(OnPlayerUpdated);
        }
    }
}