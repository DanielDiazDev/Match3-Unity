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

        public void Setup(PowerUpSO data, int amount)
        {
            _icon.sprite = data.iconUI;
            _amountText.text = amount.ToString();
        }
    }
}