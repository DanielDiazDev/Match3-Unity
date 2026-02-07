using TMPro;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

namespace Match3.Scripts.Shop.Item
{
    public class ShopItemView : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI _name;
        [SerializeField] private TextMeshProUGUI _price;
        [SerializeField] private Image _icon;
        [SerializeField] private Button _button;

        private CompositeDisposable _disposables = new();

        public void Bind(ShopItemViewModel model)
        {
            model.Name.Subscribe(x => _name.text = x).AddTo(_disposables);
            model.PriceText.Subscribe(x => _price.text = x).AddTo(_disposables);
            model.Icon.Subscribe(x => _icon.sprite = x).AddTo(_disposables);

            _button.onClick.AddListener(() => model.Clicked.Execute());
        }

        private void OnDestroy()
        {
            _disposables.Dispose();
        }
    }
}