using System.Collections.Generic;
using System.Linq;
using Assets.Match3.Scripts.Grid;
using Match3.Scripts.Grid;
using Match3.Scripts.Match.Systems.Find;
using Match3.Scripts.Pieces;
using UnityEngine;

namespace Match3.Scripts.Match.Systems
{
    public class ShuffleBoardSystem
    {
        private readonly GridSystem<GridObject> _gridSystem;
        private readonly MatchFinder _matchFinder;
        private readonly MoveValidator _moveValidator;

        private const int MAX_ATTEMPTS = 50;

        public ShuffleBoardSystem(
            GridSystem<GridObject> gridSystem,
            MatchFinder matchFinder,
            MoveValidator moveValidator)
        {
            _gridSystem = gridSystem;
            _matchFinder = matchFinder;
            _moveValidator = moveValidator;
        }

        public void Shuffle()
        {
            var allPieces = CollectPieces();
            var positions = _gridSystem.GetAllPositions();

            for (int attempt = 0; attempt < MAX_ATTEMPTS; attempt++)
            {
                ShuffleList(allPieces);

                PlacePieces(allPieces, positions);

                if (_matchFinder.FindMatches(positions).Any())
                    continue;

                if (!_moveValidator.HasAnyValidMove())
                    continue;

                return;
            }

            Debug.LogError("Shuffle has no valid moves.");
        }

        private List<Piece> CollectPieces()
        {
            var result = new List<Piece>();

            foreach (var pos in _gridSystem.GetAllPositions())
            {
                var cell = _gridSystem.GetValue(pos.x, pos.y);
                if (!cell.IsEmpty())
                {
                    result.Add(cell.GetPiece());
                    cell.Clear();
                }
            }

            return result;
        }

        private void PlacePieces(List<Piece> pieces, List<Vector2Int> positions)
        {
            int index = 0;

            foreach (var pos in positions)
            {
                var cell = _gridSystem.GetValue(pos.x, pos.y);
                var piece = pieces[index++];

                cell.SetPiece(piece);
                piece.transform.position =
                    _gridSystem.GetWorldPositionCenter(pos.x, pos.y);
            }
        }

        private void ShuffleList<T>(List<T> list)
        {
            for (int i = 0; i < list.Count; i++)
            {
                int rnd = Random.Range(i, list.Count);
                (list[i], list[rnd]) = (list[rnd], list[i]);
            }
        }
    }
}