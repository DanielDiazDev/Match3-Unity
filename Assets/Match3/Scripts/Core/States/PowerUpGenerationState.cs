using Cysharp.Threading.Tasks;
using Match3.Scripts.General;

namespace Match3.Scripts.Core.States
{
    /// <summary>
    /// Gestiona la generación de power-ups derivada de coincidencias (4 en línea, L, T, etc.).
    /// El MatchProcessor ya instaura los power-ups en las celdas; este estado espera a que
    /// se completen los comandos y pasa a Gravity.
    /// </summary>
    public class PowerUpGenerationState : IMatchState
    {
        private readonly MatchStateContext _ctx;

        public PowerUpGenerationState(MatchStateContext ctx)
        {
            _ctx = ctx;
        }

        public void Enter()
        {
            _ctx.SetBusy(true);
        }

        public async UniTask<MatchState> Execute()
        {
            // Esperar a que finalicen los comandos de generación de power-ups
            await CommandQueue.Instance.WaitForAllCommands();
            await UniTask.Yield();

            return MatchState.Gravity;
        }

        public void Exit()
        {
            _ctx.SetBusy(false);
        }
    }
}
