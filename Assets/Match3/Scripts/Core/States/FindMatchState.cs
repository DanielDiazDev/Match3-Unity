using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using Match3.Scripts.Match.Systems.Find;
using Match3.Scripts.Sound;
using UnityEngine;

namespace Match3.Scripts.Core.States
{
    /// <summary>
    /// Se usa tras Gravity/Refill (cascade) para buscar nuevas coincidencias en todo el tablero.
    /// Si hay matches, las guarda en ctx.LastMatches y pasa a Destroy. Si no, vuelve a Idle.
    /// </summary>
    public class FindMatchState : IMatchState
    {
        private readonly MatchStateContext _ctx;

        public FindMatchState(MatchStateContext ctx)
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
            // Buscar matches en toda la grilla (detectar cascade tras relleno)
            var matches = _ctx.MatchFinder.FindMatches(_ctx.GridSystem.GetAllPositions());

            if (matches.Count == 0)
            {
                Debug.Log("[FindMatchState] No hay matches en cascade.");
                return MatchState.Idle;
            }

            // Multiplicador de cascada
            _ctx.ScoreSystem.IncrementMultiplier();

            _ctx.LastMatches = matches;
            return MatchState.Destroy;
        }

        public void Exit()
        {
            _ctx.SetBusy(false);
        }
    }
}
