using TMPro;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

namespace Match3.Scripts.Level
{
    public class LevelDetailView : ViewBase
    {
        [SerializeField] private TextMeshProUGUI _levelIdText;
        [SerializeField] private Image[] _starImages;
        [SerializeField] private Sprite _starOn;
        [SerializeField] private Sprite _starOff;
        [SerializeField] private Button _startButton;
        [SerializeField] private GameObject _panelRoot;

        private LevelDetailViewModel _levelDetailViewModel;

        public void Configure(LevelDetailViewModel levelDetailViewModel)
        {
            _levelDetailViewModel = levelDetailViewModel;
            _levelDetailViewModel.LevelId.Subscribe(id => _levelIdText.text = id).AddTo(_disposables);
            _levelDetailViewModel.Stars.Subscribe(UpdateStars).AddTo(_disposables);
            _startButton.onClick.AddListener(() => _levelDetailViewModel.StartLevel.Execute());
        }

        private void UpdateStars(int stars)
        {
            for (int i = 0; i < _starImages.Length; i++) _starImages[i].sprite = i < stars ? _starOn : _starOff;
        }

        public void Show() => _panelRoot.SetActive(true);

        public void Hide() => _panelRoot.SetActive(false);
    }
}