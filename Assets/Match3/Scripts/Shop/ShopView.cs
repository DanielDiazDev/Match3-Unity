using Match3.Scripts.Shop.Item;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

namespace Match3.Scripts.Shop
{
    public class ShopView : MonoBehaviour
    {
        [SerializeField] private Button _powerUpsTab;
        [SerializeField] private Button _statsTab;
        [SerializeField] private Button _closeButton;
        [SerializeField] private Transform _content;
        [SerializeField] private ShopItemView _itemPrefab;

        private ShopViewModel _model;
        private CompositeDisposable _disposables = new();

        public void Configure(ShopViewModel model)
        {
            _model = model;

            _powerUpsTab.onClick.AddListener(() => _model.SelectPowerUpsTab.Execute());
            _statsTab.onClick.AddListener(() => _model.SelectStatsTab.Execute());
            _closeButton.onClick.AddListener(Hide);

            _model.VisibleItems
                .ObserveAdd()
                .Subscribe(x => CreateItem(x.Value))
                .AddTo(_disposables);

            _model.VisibleItems
                .ObserveReset()
                .Subscribe(_ => Clear())
                .AddTo(_disposables);
        }

        public void Show()
        {
            gameObject.SetActive(true);
            _closeButton.gameObject.SetActive(true);
            
        }

        public void Hide()
        {
            Level.LevelManager.Instance.SetLevelSelectorInteractable(true);
            gameObject.SetActive(false);
            _closeButton.gameObject.SetActive(false);
        }

        private void CreateItem(ShopItemViewModel vm)
        {
            var view = Instantiate(_itemPrefab, _content);
            view.Bind(vm);
        }

        private void Clear()
        {
            foreach (Transform child in _content)
                Destroy(child.gameObject);
        }
    }
}