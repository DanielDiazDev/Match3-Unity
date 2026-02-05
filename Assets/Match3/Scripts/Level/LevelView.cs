using System;
using TMPro;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

namespace Match3.Scripts.Level
{
    public class LevelView : ViewBase
    {
        [SerializeField] private TextMeshProUGUI _levelIdText;
        [SerializeField] private Image[] _starImages; 
        [SerializeField] private Sprite _starOn; 
        [SerializeField] private Sprite _starOff; 
        [SerializeField] private Button _levelButton;
        private LevelViewModel  _levelViewModel;

        public void Configure(LevelViewModel levelViewModel)
        {
            _levelViewModel = levelViewModel;
            _levelViewModel.LevelId
                .Subscribe(id => _levelIdText.SetText(id))
                .AddTo(_disposables);
            _levelViewModel.Stars.Subscribe(UpdateStars).AddTo(_disposables); 
            _levelViewModel.Unlocked.Subscribe(unlocked => _levelButton.interactable = unlocked).AddTo(_disposables);
            _levelButton.onClick.AddListener(() =>
            {
                _levelViewModel.LevelButtonPressed.Execute(_levelViewModel.LevelId.Value);
                LevelManager.Instance.ShowLevelDetail();
            });
        }

        private void UpdateStars(int stars)
        {
            for (int i = 0; i < _starImages.Length; i++)
            {
                _starImages[i].sprite = i < stars ? _starOn : _starOff;
            }
        }
    }
}