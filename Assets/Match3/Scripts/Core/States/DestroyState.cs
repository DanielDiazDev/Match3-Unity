using Cysharp.Threading.Tasks;
using Match3.Scripts.General;

namespace Match3.Scripts.Core.States
{
    /// <summary>
    /// Procesa las coincidencias ya encontradas (guardadas en ctx.LastMatches por SwapState
    /// o por FindMatchState en cascada): destruye piezas, asigna puntuación y genera power-ups.
    /// Luego pasa a PowerUpGeneration.
    /// </summary>
    public class DestroyState : IMatchState
    {
        private readonly MatchStateContext _ctx;

        public DestroyState(MatchStateContext ctx)
        {
            _ctx = ctx;
        }

        public void Enter()
        {
            _ctx.SetBusy(true);
        }

        public async UniTask<MatchState> Execute()
        {
            var matches = _ctx.LastMatches;

            if (matches == null || matches.Count == 0)
                return MatchState.Gravity;

            // Procesar matches (destrucción, puntuación, power-ups generados)
            await _ctx.MatchProcessor.ProcessMatches(matches);
            await CommandQueue.Instance.WaitForAllCommands();

            _ctx.ClearLastMatches();
            return MatchState.PowerUpGeneration;
        }

        public void Exit()
        {
            _ctx.SetBusy(false);
        }
    }
}
