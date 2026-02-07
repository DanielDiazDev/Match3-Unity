using System;
using UniRx;

namespace Match3.Scripts.UI.Inventory
{
    public class InventoryController : IDisposable
    {
        private readonly InventoryViewModel _viewModel;
        private readonly CompositeDisposable _disposables = new();

        public InventoryController(InventoryViewModel viewModel)
        {
            _viewModel = viewModel;

            _viewModel.PowerUps.ObserveAdd().Subscribe(x =>
            {
                x.Value.OnClicked.Subscribe(_ => OnPowerUpClicked(x.Value)).AddTo(_disposables);
            }).AddTo(_disposables);
        }

        private void OnPowerUpClicked(InventoryPowerUpViewModel clickedVm)
        {
            if (_viewModel.IsBusy.Value) return;

            if (_viewModel.SelectedPowerUpId.Value == clickedVm.Id)
            {
                // Deselect if already selected
                _viewModel.SelectedPowerUpId.Value = string.Empty;
                clickedVm.IsSelected.Value = false;
            }
            else
            {
                // Deselect others and select this one
                foreach (var vm in _viewModel.PowerUps)
                {
                    vm.IsSelected.Value = vm.Id == clickedVm.Id;
                }
                _viewModel.SelectedPowerUpId.Value = clickedVm.Id;
            }
        }

        public void Deselect()
        {
            _viewModel.SelectedPowerUpId.Value = string.Empty;
            foreach (var vm in _viewModel.PowerUps)
            {
                vm.IsSelected.Value = false;
            }
        }

        public void Dispose()
        {
            _disposables.Dispose();
        }
    }
}
