using System;
using UnityEngine;
using UnityEngine.UI;

namespace Match3.Scripts.UI.EndGame
{
    public class LoseUI : MonoBehaviour
    {
        [SerializeField] private Button _continueButton;

        public void Setup(Action onContinue)
        {
            _continueButton.onClick.RemoveAllListeners();
            _continueButton.onClick.AddListener(() => onContinue?.Invoke());
        }
    }
}
