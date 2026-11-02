using Cysharp.Threading.Tasks;
using Match3.Scripts.General;
using UnityEngine;

namespace Match3.Scripts.Core.States
{
    /// <summary>
    /// Mezcla el tablero cuando no quedan movimientos válidos.
    /// </summary>
    public class ShuffleState : IMatchState
    {
        private readonly MatchStateContext _ctx;

        public ShuffleState(MatchStateContext ctx)
        {
            _ctx = ctx;
        }

        public void Enter()
        {
            _ctx.SetBusy(true);
        }

        public async UniTask<MatchState> Execute()
        {
            Debug.Log("⚠ No hay movimientos, reshuffle necesario");
            _ctx.ShuffleBoardSystem.Shuffle();
            await CommandQueue.Instance.WaitForAllCommands();
            await UniTask.Yield();

            return MatchState.Idle;
        }

        public void Exit()
        {
            _ctx.SetBusy(false);
        }
    }
}
