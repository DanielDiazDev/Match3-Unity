using System.Linq;
using Assets.Match3.Scripts.Grid;
using Cysharp.Threading.Tasks;
using Match3.Scripts.General;
using Match3.Scripts.Grid;
using Match3.Scripts.Match.Systems;
using Match3.Scripts.Match.Systems.Find;
using Match3.Scripts.Score;

namespace Match3.Scripts.Core
{
    public class BoardResolver
    {
        private readonly GridSystem<GridObject> _gridSystem;
        private readonly IScoreSystem _scoreSystem;
        private readonly MatchFinder _matchFinder;
        private readonly MatchProcessor _matchProcessor;
        private readonly GravitySystem _gravitySystem;
        private readonly RefillSystem _refillSystem;

        public BoardResolver(
            MatchFinder matchFinder,
            MatchProcessor matchProcessor,
            GravitySystem gravitySystem,
            RefillSystem refillSystem,
            GridSystem<GridObject> gridSystem,
            IScoreSystem scoreSystem)
        {
            _matchFinder = matchFinder;
            _matchProcessor = matchProcessor;
            _gravitySystem = gravitySystem;
            _refillSystem = refillSystem;
            _gridSystem = gridSystem;
            _scoreSystem = scoreSystem;
        }

        public async UniTask Resolve(int difficulty = 1)
        {
            bool matchesFound;
            int cascadeCount = 0;

            do
            {
                var matches = _matchFinder.FindMatches(_gridSystem.GetAllPositions());
                matchesFound = matches.Any();

                if (matchesFound)
                {
                    if (cascadeCount > 0)
                    {
                        _scoreSystem.IncrementMultiplier();
                    }
                    
                    await _matchProcessor.ProcessMatches(matches);
                    await CommandQueue.Instance.WaitForAllCommands();
                }

                bool moved;
                do
                {
                    moved = await _gravitySystem.ApplyGravity();
                }
                while (moved);

                await _refillSystem.ApplyRefill(difficulty, cascadeCount);
                await UniTask.Yield();
                
                if (matchesFound) cascadeCount++;

            } while (matchesFound);
        }
    }
}