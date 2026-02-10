using Assets.Match3.Scripts.Grid;
using Match3.Scripts.Grid;
using UnityEngine;

namespace Match3.Scripts.Input
{
    public class InputController
    {
        private readonly GridSystem<GridObject> _grid;
      //  private readonly GameFlowController _gameFlow;
      //  private readonly IInventorySelectionService _inventorySelection;

        public InputController(
            GridSystem<GridObject> grid)
            // GameFlowController gameFlow,
            // IInventorySelectionService inventorySelection)
        {
            _grid = grid;
           // _gameFlow = gameFlow;
         //   _inventorySelection = inventorySelection;
        }

        public void HandleSwipe(Vector2 worldPos, Vector2Int direction)
        {
            // if (_gameFlow.IsBusy)
            //     return;

            var from = _grid.GetXY(worldPos);
            var to = from + direction;

            if (!IsValid(from) || !IsValid(to))
                return;

          //  _gameFlow.TrySwap(from, to).Forget();
        }

        public void HandleClick(Vector2 worldPos)
        {
            // if (_gameFlow.IsBusy)
            //     return;

            var gridPos = _grid.GetXY(worldPos);

            if (!IsValid(gridPos))
                return;

            // if (_inventorySelection.HasSelection)
            // {
            //     _gameFlow.UsePowerUp(_inventorySelection.SelectedId, gridPos).Forget();
            //     _inventorySelection.Clear();
            // }
        }

        private bool IsValid(Vector2Int pos)
        {
            return _grid.IsValid(pos.x, pos.y);
        }
    }
}