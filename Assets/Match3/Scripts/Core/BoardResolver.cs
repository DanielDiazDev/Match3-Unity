using System.Linq;
using Assets.Match3.Scripts.Grid;
using Cysharp.Threading.Tasks;
using Match3.Scripts.General;
using Match3.Scripts.Grid;
using Match3.Scripts.Match.Systems;
using Match3.Scripts.Match.Systems.Find;

namespace Match3.Scripts.Core
{
    public class BoardResolver
    {
        private readonly MatchFinder _matchFinder;
        private readonly MatchProcessor _matchProcessor;
        private readonly GravitySystem _gravitySystem;
        private readonly RefillSystem _refillSystem;
        private readonly GridSystem<GridObject> _gridSystem;

        public BoardResolver(
            MatchFinder matchFinder,
            MatchProcessor matchProcessor,
            GravitySystem gravitySystem,
            RefillSystem refillSystem,
            GridSystem<GridObject> gridSystem)
        {
            _matchFinder = matchFinder;
            _matchProcessor = matchProcessor;
            _gravitySystem = gravitySystem;
            _refillSystem = refillSystem;
            _gridSystem = gridSystem;
        }

        public async UniTask Resolve()
        {
            bool matchesFound;

            do
            {
                var matches = _matchFinder.FindMatches(_gridSystem.GetAllPositions());
                matchesFound = matches.Any();

                if (matchesFound)
                {
                    await _matchProcessor.ProcessMatches(matches);
                    await CommandQueue.Instance.WaitForAllCommands();
                }

                bool moved;
                do
                {
                    moved = await _gravitySystem.ApplyGravity();
                }
                while (moved);

                await _refillSystem.ApplyRefill();
                await UniTask.Yield();

            } while (matchesFound);
        }
    }
}