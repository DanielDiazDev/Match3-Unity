using System.Collections.Generic;
using System.Linq;
using Match3.Scripts.Data.Item;
using Match3.Scripts.Persistence;

namespace Match3.Scripts.UI.Inventory
{
    public class InventoryPresenter
    {
        private readonly InventoryViewModel _viewModel;
        private readonly IProgressRepository _progressRepository;
        private readonly PowerUpSODatabase _powerUpDatabase;

        public InventoryPresenter(InventoryViewModel viewModel, IProgressRepository progressRepository, PowerUpSODatabase powerUpDatabase)
        {
            _viewModel = viewModel;
            _progressRepository = progressRepository;
            _powerUpDatabase = powerUpDatabase;
        }

        public void RefreshInventory()
        {
            _viewModel.PowerUps.Clear();
            var purchased = _progressRepository.GetPurchasedPowerUps();

            foreach (var kvp in purchased)
            {
                var powerUpSo = _powerUpDatabase.powerUpSOs.FirstOrDefault(p => p.id == kvp.Key);
                if (powerUpSo != null)
                {
                    var itemViewModel = new InventoryPowerUpViewModel(kvp.Key, powerUpSo.sprite, kvp.Value);
                    _viewModel.PowerUps.Add(itemViewModel);
                }
            }
        }
    }
}
