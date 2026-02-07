using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UniRx;

namespace Match3.Scripts.UI.Inventory
{
    public class InventoryPowerUpItemView : MonoBehaviour
    {
        [SerializeField] private Image _iconImage;
        [SerializeField] private TextMeshProUGUI _quantityText;
        [SerializeField] private GameObject _selectionHighlight;
        [SerializeField] private Button _button;

        private CompositeDisposable _disposables = new();

        public void Bind(InventoryPowerUpViewModel viewModel)
        {
            _disposables.Clear();
            _iconImage.sprite = viewModel.Icon;
            
            viewModel.Quantity
                .Subscribe(q => _quantityText.SetText(q.ToString()))
                .AddTo(_disposables);

            viewModel.IsSelected
                .Subscribe(selected => _selectionHighlight.SetActive(selected))
                .AddTo(_disposables);

            _button.onClick.AddListener(() => viewModel.OnClicked.Execute());
        }

        private void OnDestroy()
        {
            _disposables.Dispose();
        }
    }
}
