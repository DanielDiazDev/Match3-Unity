using Cysharp.Threading.Tasks;

namespace Match3.Scripts.Core.States
{
    /// <summary>
    /// Aplica gravedad: las piezas caen para llenar los espacios vacíos.
    /// Repite hasta que no se mueva nada más.
    /// </summary>
    public class GravityState : IMatchState
    {
        private readonly MatchStateContext _ctx;

        public GravityState(MatchStateContext ctx)
        {
            _ctx = ctx;
        }

        public void Enter()
        {
            _ctx.SetBusy(true);
        }

        public async UniTask<MatchState> Execute()
        {
            bool moved;
            do
            {
                moved = await _ctx.GravitySystem.ApplyGravity();
            }
            while (moved);

            await UniTask.Yield();
            return MatchState.Refill;
        }

        public void Exit()
        {
            _ctx.SetBusy(false);
        }
    }
}
