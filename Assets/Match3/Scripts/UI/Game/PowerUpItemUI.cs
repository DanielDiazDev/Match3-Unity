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
        [SerializeField] private TextMeshProUGUI _amountText;
        [SerializeField] private Button _button;
        [SerializeField] private GameObject _selectionHighlight;

        public string Id => _powerUpId;
        public bool IsEmpty => _isEmpty;

        private string _powerUpId;
        private bool _isEmpty;

        public Action<string> OnClicked;

        public void Setup(PowerUpSO data, int amount)
        {
            _isEmpty = false;
            _powerUpId = data.id;

            _icon.sprite = data.iconUI != null ? data.iconUI : data.sprite;

            if (_amountText != null)
            {
                _amountText.gameObject.SetActive(true);
                _amountText.text = amount.ToString();
            }

            _button.interactable = amount > 0;
            _selectionHighlight.SetActive(false);

            _button.onClick.RemoveAllListeners();
            _button.onClick.AddListener(() =>
            {
                if (!_isEmpty)
                    OnClicked?.Invoke(_powerUpId);
            });
        }

        public void SetupEmpty(Sprite emptySprite)
        {
            _isEmpty = true;
            _powerUpId = null;

            _icon.sprite = emptySprite;

            if (_amountText != null)
                _amountText.gameObject.SetActive(false);

            _button.interactable = false;
            _selectionHighlight.SetActive(false);

            _button.onClick.RemoveAllListeners();
        }

        public void SetSelected(bool selected)
        {
            if (_isEmpty)
            {
                _selectionHighlight.SetActive(false);
                return;
            }

            _selectionHighlight.SetActive(selected);
        }
    }
}