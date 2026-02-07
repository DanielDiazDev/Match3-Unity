using System;
using System.Collections.Generic;
using System.Linq;
using Match3.Scripts.Data.Shop;
using Match3.Scripts.Persistence;
using Match3.Scripts.Player;
using Match3.Scripts.Shop.Item;
using Match3.Scripts.Utils;
using UnityEngine;

namespace Match3.Scripts.Shop
{
    public class ShopManager : MonoBehaviour
    {
        [SerializeField] private ShopView _view;
        [SerializeField] private List<ShopItemSO> _items;
        [SerializeField] private PlayerView _playerView;

        private PlayerModel _player;
        private List<IDisposable> _disposables = new();

        private void Start()
        {
            var progressRepository = ServiceLocator.ServiceLocator.Global.Get<IProgressRepository>();
            var gameData = progressRepository.Load();
            _player = new PlayerModel(gameData.coins, gameData.hearts);

            var playerViewModel = new PlayerViewModel();
            _playerView.Configure(playerViewModel);

            var playerPresenter = new PlayerPresenter(playerViewModel);
            _disposables.Add(playerPresenter);

            EventDispatcherService.Instance.Dispatch(_player);

            var shopItems = _items
                .Select(x => new ShopItemModel(x))
                .ToList();

            var shopViewModel = new ShopViewModel();
            _view.Configure(shopViewModel);

            var shopPresenter = new ShopPresenter(shopViewModel, shopItems);
            _disposables.Add(shopPresenter);

            var shopUseCase = new ShopUseCase(_player);

            var shopController = new ShopController(
                shopViewModel,
                shopPresenter,
                shopUseCase);

            _disposables.Add(shopController);
        }

        public void OpenShop()
        {
            _view.Show();
        }

        private void OnDestroy()
        {
            foreach (var disposable in _disposables)
                disposable.Dispose();
        }
    }
}