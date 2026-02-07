using UniRx;
using System.Collections.Generic;

namespace Match3.Scripts.UI.Inventory
{
    public class InventoryViewModel
    {
        public ReactiveCollection<InventoryPowerUpViewModel> PowerUps { get; }
        public ReactiveProperty<string> SelectedPowerUpId { get; }
        public ReactiveProperty<bool> IsBusy { get; }

        public InventoryViewModel()
        {
            PowerUps = new ReactiveCollection<InventoryPowerUpViewModel>();
            SelectedPowerUpId = new ReactiveProperty<string>(string.Empty);
            IsBusy = new ReactiveProperty<bool>(false);
        }
    }
}
