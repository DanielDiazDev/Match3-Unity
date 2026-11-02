using Cysharp.Threading.Tasks;
using Match3.Scripts.Event;
using Match3.Scripts.Event.Data;
using Match3.Scripts.General;
using UnityEngine;

namespace Match3.Scripts.Core.States
{
    /// <summary>
    /// Estado terminal: partida perdida (se agotaron los movimientos).
    /// </summary>
    public class GameOverState : IMatchState
    {
        private readonly MatchStateContext _ctx;

        public GameOverState(MatchStateContext ctx)
        {
            _ctx = ctx;
        }

        public void Enter()
        {
            _ctx.SetBusy(true);
            _ctx.GameEnded = true;
        }

        public async UniTask<MatchState> Execute()
        {
            Debug.Log("[MatchManager] Game Over! Moves finished without completing objectives.");
            await CommandQueue.Instance.WaitForAllCommands();
            if (_ctx.GameEnd != null)
                _ctx.GameEnd.SetActive(true);

            var showLoseEventData = new ShowLoseEventData(
                EventIds.ShowLose,
                _ctx.ScoreSystem.CurrentScore);
            EventQueue.Instance.EnqueueEvent(showLoseEventData);

            await CommandQueue.Instance.WaitForAllCommands();
            return MatchState.None;
        }

        public void Exit()
        {
            _ctx.SetBusy(false);
        }
    }
}
