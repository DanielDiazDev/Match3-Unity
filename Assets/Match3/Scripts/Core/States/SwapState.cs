using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using Match3.Scripts.General;
using Match3.Scripts.Match.Systems.Find;
using Match3.Scripts.Sound;
using UnityEngine;

namespace Match3.Scripts.Core.States
{
    /// <summary>
    /// Intercambia dos piezas adyacentes. Si una tiene power-up, lo dispara.
    /// Comprueba si hay matches tras el intercambio. Si no los hay (y no hay power-up),
    /// revierte el intercambio y vuelve a Idle. Si los hay, guarda los matches y pasa a Destroy.
    /// </summary>
    public class SwapState : IMatchState
    {
        private readonly MatchStateContext _ctx;

        public SwapState(MatchStateContext ctx)
        {
            _ctx = ctx;
        }

        public void Enter()
        {
            _ctx.SetBusy(true);
            _ctx.ClearLastMatches();
        }

        public async UniTask<MatchState> Execute()
        {
            if (!_ctx.PendingSwap.HasValue)
            {
                Debug.LogWarning("[SwapState] No hay intercambio pendiente.");
                return MatchState.Idle;
            }

            var (a, b) = _ctx.PendingSwap.Value;
            _ctx.ClearPendingSwap();

            // Reset multiplicador al iniciar un nuevo turno del jugador
            _ctx.ScoreSystem.ResetMultiplier();

            if (!IsValid(a) || !IsValid(b))
                return MatchState.Idle;

            if (IsEmpty(a) || IsEmpty(b))
                return MatchState.Idle;

            // Sonido y animación de intercambio
            ServiceLocator.ServiceLocator.Global.Get<SoundManager>().PlaySFX(SoundId.Swap);
            await _ctx.PieceSwapper.SwapTile(a, b);

            bool powerUpTriggered = false;

            var cellA = _ctx.GridSystem.GetValue(a.x, a.y);
            var cellB = _ctx.GridSystem.GetValue(b.x, b.y);

            if (cellA != null && cellA.HasPowerUp())
            {
                _ctx.PowerUpSystem.TriggerPowerUp(a);
                powerUpTriggered = true;
            }

            if (cellB != null && cellB.HasPowerUp())
            {
                _ctx.PowerUpSystem.TriggerPowerUp(b);
                powerUpTriggered = true;
            }

            if (powerUpTriggered)
                await CommandQueue.Instance.WaitForAllCommands();

            // Buscar matches en las posiciones afectadas
            var matches = _ctx.MatchFinder.FindMatches(new List<Vector2Int> { a, b });

            if (matches.Count == 0 && !powerUpTriggered)
            {
                // No hay coincidencias: revertir el intercambio
                ServiceLocator.ServiceLocator.Global.Get<SoundManager>().PlaySFX(SoundId.Swap);
                await _ctx.PieceSwapper.SwapTile(b, a);
                return MatchState.Idle;
            }

            // Guardar matches para que DestroyState los procese
            _ctx.LastMatches = matches;

            // Descontar movimiento solo si el intercambio fue válido (hubo match o power-up)
            _ctx.RemainingMoves--;
            _ctx.NotifyMovesChanged(_ctx.RemainingMoves);

            return MatchState.Destroy;
        }

        public void Exit()
        {
            _ctx.SetBusy(false);
        }

        private bool IsValid(Vector2Int p) =>
            p.x >= 0 && p.x < _ctx.LevelDataSO.width && p.y >= 0 && p.y < _ctx.LevelDataSO.height;

        private bool IsEmpty(Vector2Int p) =>
            _ctx.GridSystem.GetValue(p.x, p.y)?.GetPiece() == null;
    }
}
