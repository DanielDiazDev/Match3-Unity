using System.Collections.Generic;
using System.Linq;
using Assets.Match3.Scripts.Grid;
using Cysharp.Threading.Tasks;
using Match3.Scripts.General;
using Match3.Scripts.Grid;
using Match3.Scripts.Match.Systems;
using Match3.Scripts.Match.Systems.Find;
using Match3.Scripts.PowerUps;
using Match3.Scripts.Score;
using UnityEngine;

namespace Match3.Scripts.Core
{
    public class TurnFlowController
    {
        private readonly PieceSwapper _swapper;
        private readonly MoveValidator _validator;
        private readonly BoardResolver _resolver;
        private readonly PowerUpSystem _powerUpSystem;
        private readonly IScoreSystem _scoreSystem;
        private readonly GridSystem<GridObject> _gridSystem;
        private readonly MatchFinder _matchFinder;

        public TurnFlowController(
            PieceSwapper swapper,
            MoveValidator validator,
            BoardResolver resolver,
            PowerUpSystem powerUpSystem,
            IScoreSystem scoreSystem, GridSystem<GridObject> gridSystem, MatchFinder matchFinder)
        {
            _swapper = swapper;
            _validator = validator;
            _resolver = resolver;
            _powerUpSystem = powerUpSystem;
            _scoreSystem = scoreSystem;
            _gridSystem = gridSystem;
            _matchFinder = matchFinder;
        }

        public async UniTask<bool> ExecuteMove(Vector2Int a, Vector2Int b)
        {
            _scoreSystem.ResetMultiplier();

            await _swapper.SwapTile(a, b);

            bool powerUpTriggered = false;

            var cellA = _gridSystem.GetValue(a.x, a.y);
            var cellB = _gridSystem.GetValue(b.x, b.y);

            if (cellA.HasPowerUp())
            {
                _powerUpSystem.TriggerPowerUp(a);
                powerUpTriggered = true;
            }

            if (cellB.HasPowerUp())
            {
                _powerUpSystem.TriggerPowerUp(b);
                powerUpTriggered = true;
            }

            if (powerUpTriggered)
            {
                await CommandQueue.Instance.WaitForAllCommands();
            }

            var matches = _matchFinder.FindMatches(new List<Vector2Int> { a, b });

            if (!matches.Any() && !powerUpTriggered)
            {
                await _swapper.SwapTile(b, a);
                return false;
            }

            // await Resolve();
            return true;
        }

        public async UniTask Resolve(int difficulty = 1) => await _resolver.Resolve(difficulty);
    }
}