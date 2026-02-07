using UniRx;
using UnityEngine;

namespace Match3.Scripts.UI.Inventory
{
    public class InventoryPowerUpViewModel
    {
        public string Id { get; }
        public Sprite Icon { get; }
        public ReactiveProperty<int> Quantity { get; }
        public ReactiveProperty<bool> IsSelected { get; }
        public ReactiveCommand OnClicked { get; }

        public InventoryPowerUpViewModel(string id, Sprite icon, int quantity)
        {
            Id = id;
            Icon = icon;
            Quantity = new ReactiveProperty<int>(quantity);
            IsSelected = new ReactiveProperty<bool>(false);
            OnClicked = new ReactiveCommand();
        }
    }
}
