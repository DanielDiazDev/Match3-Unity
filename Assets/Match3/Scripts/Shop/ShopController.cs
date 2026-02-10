using System;
using System.Collections.Generic;
using Match3.Scripts.Ads;
using Match3.Scripts.Shop.Item;
using UniRx;

namespace Match3.Scripts.Shop
{
    public class ShopController : IDisposable
    {
        private readonly ShopViewModel _model;
        private readonly ShopPresenter _presenter;
        private readonly IShopUseCase _shopUseCase;

        private CompositeDisposable _disposables = new();

        public ShopController(
            ShopViewModel model,
            ShopPresenter presenter,
            IShopUseCase shopUseCase)
        {
            _model = model;
            _presenter = presenter;
            _shopUseCase = shopUseCase;

            _model.SelectPowerUpsTab
                .Subscribe(_ => _model.SelectedCategory.Value = ShopCategoryType.PowerUps)
                .AddTo(_disposables);

            _model.SelectStatsTab
                .Subscribe(_ => _model.SelectedCategory.Value = ShopCategoryType.Stats)
                .AddTo(_disposables);

            _model.VisibleItems.ObserveAdd()
                .Subscribe(x =>
                {
                    x.Value.Clicked
                        .Subscribe(_ => OnItemClicked(x.Value))
                        .AddTo(_disposables);
                })
                .AddTo(_disposables);
        }

        private void OnItemClicked(ShopItemViewModel vm)
        {
            var item = _presenter.GetItem(vm.Id);

            if (item.Category == (Item.ShopCategoryType)ShopCategoryType.PowerUps)
            {
                _shopUseCase.BuyPowerUp(item);
            }
            else
            {
                AdController.Instance.ShowAds();
                _shopUseCase.GiveStats(item);
            }
        }

        public void Dispose() => _disposables.Dispose();
    }

}