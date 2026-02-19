using Match3.Scripts.Sound;
using UnityEngine;
using UnityEngine.UI;

namespace Match3.Scripts.General
{
    public class MuteSound : MonoBehaviour
    {
        [SerializeField] private Button _soundButton;
        [SerializeField] private Image _icon;
        [SerializeField] private Sprite _soundOnSprite;
        [SerializeField] private Sprite _soundOffSprite;

        private SoundManager _soundManager;

        private void Start()
        {
            _soundManager = ServiceLocator.ServiceLocator.Global.Get<SoundManager>();

            _soundButton.onClick.AddListener(OnClick);

            _soundManager.OnMuteChanged += UpdateIcon;

            UpdateIcon(_soundManager.IsMuted);
        }

        private void OnDestroy()
        {
            if (_soundManager != null)
                _soundManager.OnMuteChanged -= UpdateIcon;
        }

        private void OnClick()
        {
            _soundManager.ToggleMute();
        }

        private void UpdateIcon(bool isMuted)
        {
            _icon.sprite = isMuted ? _soundOffSprite : _soundOnSprite;
        }
    }
}