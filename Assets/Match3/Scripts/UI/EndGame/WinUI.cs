using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Match3.Scripts.UI.EndGame
{
    public class WinUI : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI _scoreText;
        [SerializeField] private Image[] _starImages; 
        [SerializeField] private Sprite _starOn; 
        [SerializeField] private Sprite _starOff; 
        [SerializeField] private Button _continueButton;

        public void Setup(int score, int stars, Action onContinue)
        {
            _scoreText.text = score.ToString();

            for (int i = 0; i < _starImages.Length; i++)
            {
                _starImages[i].sprite = i < stars ? _starOn : _starOff;
            }

            _continueButton.onClick.RemoveAllListeners();
            _continueButton.onClick.AddListener(() => onContinue?.Invoke());
        }
    }
}
