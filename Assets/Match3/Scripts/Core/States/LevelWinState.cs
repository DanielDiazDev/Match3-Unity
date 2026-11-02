using Cysharp.Threading.Tasks;
using Match3.Scripts.Event;
using Match3.Scripts.Event.Data;
using Match3.Scripts.General;
using UnityEngine;

namespace Match3.Scripts.Core.States
{
    /// <summary>
    /// Estado terminal: nivel completado (objetivos alcanzados).
    /// </summary>
    public class LevelWinState : IMatchState
    {
        private readonly MatchStateContext _ctx;

        public LevelWinState(MatchStateContext ctx)
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
            // Sincronizar estrellas antes de guardar
            _ctx.StarSystem.UpdateStarPower();
            _ctx.StarSystem.ForceMinimumStar();

            _ctx.ProgressRepository.UpdateLevelProgress(
                _ctx.LevelDataSO.id,
                _ctx.StarSystem.CurrentStars,
                true);

            if (_ctx.RemainingMoves > 0 && _ctx.EndGameController != null)
            {
                await _ctx.EndGameController.ExecutePostGame(
                    _ctx.RemainingMoves,
                    _ctx.LevelDataSO.difficulty,
                    moves =>
                    {
                        _ctx.RemainingMoves = moves;
                        EventQueue.Instance.EnqueueEvent(new MovesChangedEventData(EventIds.MovesChanged, _ctx.RemainingMoves, 1));
                    });

                _ctx.StarSystem.UpdateStarPower();
                _ctx.ProgressRepository.UpdateLevelProgress(
                    _ctx.LevelDataSO.id,
                    _ctx.StarSystem.CurrentStars,
                    true);
            }

            var showWinEventData = new ShowWinEventData(
                EventIds.ShowWin,
                _ctx.ScoreSystem.CurrentScore);

            await CommandQueue.Instance.WaitForAllCommands();
            if (_ctx.GameEnd != null)
                _ctx.GameEnd.SetActive(true);
            EventQueue.Instance.EnqueueEvent(showWinEventData);

            return MatchState.None;
        }

        public void Exit()
        {
            _ctx.SetBusy(false);
        }
    }
}
