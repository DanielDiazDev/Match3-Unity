using UnityEngine;
using UniRx;
using System.Collections.Generic;

namespace Match3.Scripts.UI.Inventory
{
    public class InventoryView : MonoBehaviour
    {
        [SerializeField] private Transform _content;
        [SerializeField] private InventoryPowerUpItemView _itemPrefab;

        private CompositeDisposable _disposables = new();

        public void Configure(InventoryViewModel viewModel)
        {
            _disposables.Clear();

            viewModel.PowerUps.ObserveAdd().Subscribe(x =>
            {
                var view = Instantiate(_itemPrefab, _content);
                view.Bind(x.Value);
            }).AddTo(_disposables);

            viewModel.PowerUps.ObserveReset().Subscribe(_ =>
            {
                foreach (Transform child in _content)
                    Destroy(child.gameObject);
            }).AddTo(_disposables);
        }

        private void OnDestroy()
        {
            _disposables.Dispose();
        }
    }
}
