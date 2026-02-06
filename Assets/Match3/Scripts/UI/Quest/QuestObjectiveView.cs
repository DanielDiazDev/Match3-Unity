using TMPro;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

namespace Match3.Scripts.Quest
{
    public class QuestObjectiveView : ViewBase
    {
        [SerializeField] private Image _icon;
        [SerializeField] private TextMeshProUGUI _progressText;
        [SerializeField] private GameObject _completedMark;

        public void Configure(QuestObjectiveViewModel viewModel)
        {
            viewModel.Icon
                .Subscribe(sprite => _icon.sprite = sprite)
                .AddTo(_disposables);

            viewModel.ProgressText
                .Subscribe(text => _progressText.SetText(text))
                .AddTo(_disposables);

            viewModel.IsCompleted
                .Subscribe(completed => _completedMark.SetActive(completed))
                .AddTo(_disposables);
        }
    }
}