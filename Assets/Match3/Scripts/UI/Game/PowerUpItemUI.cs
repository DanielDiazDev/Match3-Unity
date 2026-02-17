using System;
using Match3.Scripts.Data.Item;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Match3.Scripts.UI.Game
{
    public class PowerUpItemUI : MonoBehaviour
    {
        [SerializeField] private Image _icon;
        [SerializeField] private TMP_Text _amountText;
        [SerializeField] private Button _button;
        [SerializeField] private GameObject _selectionHighlight;
        public string Id => _powerUpId;
        private string _powerUpId;

        public Action<string> OnClicked;

        public void Setup(PowerUpSO data, int amount)
        {
            _powerUpId = data.id;

            _icon.sprite = data.iconUI;
            _amountText.text = amount.ToString();

            _button.interactable = amount > 0;

            _selectionHighlight.SetActive(false);

            _button.onClick.RemoveAllListeners();
            _button.onClick.AddListener(() =>
            {
                OnClicked?.Invoke(_powerUpId);
            });
        }

        public void SetSelected(bool selected)
        {
            _selectionHighlight.SetActive(selected);
        }
    }
}