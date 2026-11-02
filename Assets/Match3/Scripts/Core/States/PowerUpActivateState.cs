using Cysharp.Threading.Tasks;
using Match3.Scripts.General;
using UnityEngine;

namespace Match3.Scripts.Core.States
{
    /// <summary>
    /// Activa un power-up seleccionado por el jugador en una posición del tablero.
    /// </summary>
    public class PowerUpActivateState : IMatchState
    {
        private readonly MatchStateContext _ctx;

        public PowerUpActivateState(MatchStateContext ctx)
        {
            _ctx = ctx;
        }

        public void Enter()
        {
            _ctx.SetBusy(true);
        }

        public async UniTask<MatchState> Execute()
        {
            if (!_ctx.PendingPowerUpPos.HasValue)
            {
                Debug.LogWarning("[PowerUpActivateState] No hay posición de power-up pendiente.");
                _ctx.ClearPendingPowerUp();
                return MatchState.Idle;
            }

            var pos = _ctx.PendingPowerUpPos.Value;
            _ctx.ClearPendingPowerUp();

            var gridObject = _ctx.GridSystem.GetValue(pos.x, pos.y);
            if (gridObject == null || gridObject.IsEmpty() || !gridObject.IsPlayable())
                return MatchState.Idle;

            // Aplicar el power-up seleccionado
            _ctx.PowerUpSystem.ChangeToPowerUp(_ctx.SelectedPowerUpId, pos);

            await UniTask.Yield();
            _ctx.PowerUpSystem.TriggerPowerUp(pos);
            await CommandQueue.Instance.WaitForAllCommands();

            // Consumir el power-up del inventario
            _ctx.ProgressRepository.ConsumePowerUp(_ctx.SelectedPowerUpId);
            _ctx.SelectedPowerUpId = null;

            // Refrescar el inventario DESPUÉS de consumir para que se vea la cantidad actualizada
            _ctx.InventoryUI.Refresh();

            return MatchState.Gravity;
        }

        public void Exit()
        {
            _ctx.SetBusy(false);
        }
    }
}
