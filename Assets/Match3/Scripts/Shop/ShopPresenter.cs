using System;
using System.Collections.Generic;
using System.Linq;
using Match3.Scripts.Data.Shop;
using Match3.Scripts.Shop.Item;
using UnityEngine;
using UniRx;

namespace Match3.Scripts.Shop
{
    public class ShopPresenter : IDisposable
    {
        private readonly ShopViewModel _model;
        private readonly List<ShopItemModel> _items;
        private readonly Dictionary<string, ShopItemModel> _itemLookup;

        private CompositeDisposable _disposables = new();

        public ShopPresenter(
            ShopViewModel model,
            List<ShopItemModel> items)
        {
            _model = model;
            _items = items;
            _itemLookup = items.ToDictionary(x => x.Id);

            _model.SelectedCategory
                .Subscribe(UpdateCategory)
                .AddTo(_disposables);

            UpdateCategory(_model.SelectedCategory.Value);
        }

        private void UpdateCategory(ShopCategoryType category)
        {
            _model.VisibleItems.Clear();

            foreach (var item in _items.Where(x => x.Category == (Item.ShopCategoryType)category))
            {
                var vm = new ShopItemViewModel(item, item.Icon);
                _model.VisibleItems.Add(vm);
            }
        }

        public ShopItemModel GetItem(string id)
            => _itemLookup[id];

        public void Dispose() => _disposables.Dispose();
    }
}