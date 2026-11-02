using Cysharp.Threading.Tasks;
using Match3.Scripts.Event;
using Match3.Scripts.Event.Data;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Match3.Scripts.Core.States
{
    /// <summary>
    /// Estado de espera: el tablero está estable y listo para recibir input del jugador.
    /// No hace nada activo; se queda hasta que MatchManager le pide transicionar (Swap o PowerUpActivate).
    /// </summary>
    public class IdleState : IMatchState
    {
        private readonly MatchStateContext _ctx;

        public IdleState(MatchStateContext ctx)
        {
            _ctx = ctx;
        }

        public void Enter()
        {
            _ctx.SetBusy(false);
        }

        public async UniTask<MatchState> Execute()
        {
            // Idle es un estado pasivo: se queda aquí hasta que MatchManager fuerza una transición.
            // Retornamos Idle para quedarnos en este estado mientras no haya input.
            await UniTask.Yield();
            return MatchState.Idle;
        }

        public void Exit()
        {
            _ctx.SetBusy(true);
        }
    }
}
