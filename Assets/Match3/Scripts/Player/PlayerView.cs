using TMPro;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

namespace Match3.Scripts.Player
{
    public class PlayerView : MonoBehaviour
    {
        private const int MaxHealth = 4;

        [Header("UI References")]
        [SerializeField] private TextMeshProUGUI _coinsText;
        [SerializeField] private Image[] _hearts; 

        [Header("Sprites")]
        [SerializeField] private Sprite _fullHeartSprite;
        [SerializeField] private Sprite _emptyHeartSprite;

        private CompositeDisposable _disposables = new();

        public void Configure(PlayerViewModel model)
        {
            model.Coins
                .Subscribe(coins => _coinsText.text = coins.ToString())
                .AddTo(_disposables);

            model.Health
                .Subscribe(UpdateHearts)
                .AddTo(_disposables);
        }

        private void UpdateHearts(int health)
        {
            health = Mathf.Clamp(health, 0, MaxHealth);

            for (int i = 0; i < _hearts.Length; i++)
            {
                _hearts[i].sprite = i < health 
                    ? _fullHeartSprite 
                    : _emptyHeartSprite;
            }
        }

        private void OnDestroy()
        {
            _disposables.Dispose();
        }
    }
}