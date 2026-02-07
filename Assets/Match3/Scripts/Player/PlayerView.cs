using TMPro;
using UniRx;
using UnityEngine;

namespace Match3.Scripts.Player
{
    public class PlayerView : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI _coinsText;
        [SerializeField] private TextMeshProUGUI _healthText;

        private CompositeDisposable _disposables = new();

        public void Configure(PlayerViewModel model)
        {
            model.Coins
                .Subscribe(coins => _coinsText.text = coins.ToString())
                .AddTo(_disposables);

            model.Health
                .Subscribe(health => _healthText.text = health.ToString())
                .AddTo(_disposables);
        }

        private void OnDestroy()
        {
            _disposables.Dispose();
        }
    }
}