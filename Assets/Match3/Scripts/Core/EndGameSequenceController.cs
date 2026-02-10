using System;
using System.Collections.Generic;
using System.Linq;
using Assets.Match3.Scripts.Grid;
using Cysharp.Threading.Tasks;
using Match3.Scripts.General;
using Match3.Scripts.Grid;
using Match3.Scripts.PowerUps;
using UnityEngine;

namespace Match3.Scripts.Core
{
    public class EndGameSequenceController
    {
        private readonly GridSystem<GridObject> _grid;
        private readonly PowerUpSystem _powerUpSystem;
        private readonly BoardResolver _resolver;

        public EndGameSequenceController(
            GridSystem<GridObject> grid,
            PowerUpSystem powerUpSystem,
            BoardResolver resolver)
        {
            _grid = grid;
            _powerUpSystem = powerUpSystem;
            _resolver = resolver;
        }

        public async UniTask ExecutePostGame(int remainingMoves, Action<int> onMoveUsed)
        {
            while (remainingMoves > 0)
            {
                remainingMoves--;
                onMoveUsed?.Invoke(remainingMoves);

                var pieces = CollectPiecesByType();
                if (pieces.Count == 0) break;

                var randomEntry = pieces.ElementAt(UnityEngine.Random.Range(0, pieces.Count));

                foreach (var pos in randomEntry.Value)
                {
                    _powerUpSystem.ClearCell(pos.x, pos.y);
                }

                await CommandQueue.Instance.WaitForAllCommands();
                await _resolver.Resolve();
            }
        }

        private Dictionary<string, List<Vector2Int>> CollectPiecesByType()
        {
            var dict = new Dictionary<string, List<Vector2Int>>();

            foreach (var pos in _grid.GetAllPositions())
            {
                var piece = _grid.GetValue(pos.x, pos.y)?.GetPiece();
                if (piece == null) continue;

                var id = piece.GetTile().id;

                if (!dict.TryGetValue(id, out var list))
                {
                    list = new List<Vector2Int>();
                    dict[id] = list;
                }

                list.Add(pos);
            }

            return dict;
        }
    }
}