using Cysharp.Threading.Tasks;
using Match3.Scripts.General;

namespace Match3.Scripts.Core.States
{
    /// <summary>
    /// Rellena las celdas vacías con nuevas piezas generadas aleatoriamente.
    /// Después de rellenar, si se crearon nuevas coincidencias, el flujo vuelve a FindMatch (cascade).
    /// Si no, vuelve a Idle.
    /// </summary>
    public class RefillState : IMatchState
    {
        private readonly MatchStateContext _ctx;

        public RefillState(MatchStateContext ctx)
        {
            _ctx = ctx;
        }

        public void Enter()
        {
            _ctx.SetBusy(true);
        }

        public async UniTask<MatchState> Execute()
        {
            await _ctx.RefillSystem.ApplyRefill(_ctx.LevelDataSO.difficulty, 0);
            await CommandQueue.Instance.WaitForAllCommands();
            await UniTask.Yield();

            // Comprobar si el relleno creó nuevas coincidencias (cascade)
            var newMatches = _ctx.MatchFinder.FindMatches(_ctx.GridSystem.GetAllPositions());
            if (newMatches.Count > 0)
                return MatchState.FindMatch;

            return MatchState.Idle;
        }

        public void Exit()
        {
            _ctx.SetBusy(false);
        }
    }
}
