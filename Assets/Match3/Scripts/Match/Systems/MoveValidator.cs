using System.Collections.Generic;
using System.Linq;
using Assets.Match3.Scripts.Grid;
using Match3.Scripts.Grid;
using Match3.Scripts.Match.Systems.Find;
using UnityEngine;

namespace Match3.Scripts.Match.Systems
{
    public class MoveValidator
    {
        private readonly GridSystem<GridObject> _gridSystem;
        private readonly MatchFinder _matchFinder;
        private readonly List<Vector2Int> _tempPositions = new List<Vector2Int>(2);

        public MoveValidator(GridSystem<GridObject> gridSystem, MatchFinder matchFinder)
        {
            _gridSystem = gridSystem;
            _matchFinder = matchFinder;
        }
        public bool HasAnyValidMove()
        {
            foreach (var pos in _gridSystem.GetAllPositions())
            {
                if (_gridSystem.GetValue(pos.x, pos.y).IsEmpty())
                    continue;

                if (CheckSwap(pos, Vector2Int.right)) return true;
                if (CheckSwap(pos, Vector2Int.up)) return true;
            }

            return false;
        }
        private bool CheckSwap(Vector2Int a, Vector2Int dir)
        {
            var b = a + dir;

            if (!_gridSystem.IsValid(b.x, b.y))
                return false;

            var cellA = _gridSystem.GetValue(a.x, a.y);
            var cellB = _gridSystem.GetValue(b.x, b.y);

            if (cellA.IsEmpty() || cellB.IsEmpty())
                return false;

            if (!cellA.GetPiece() || !cellB.GetPiece())
                return false;

            Swap(a, b);

            bool hasMatch = _matchFinder
                .FindMatches(new List<Vector2Int> { a, b })
                .Any();

            Swap(b, a);

            return hasMatch;
        }


        private void Swap(Vector2Int a, Vector2Int b)
        {
            var cellA = _gridSystem.GetValue(a.x, a.y);
            var cellB = _gridSystem.GetValue(b.x, b.y);

            var temp = cellA.GetPiece();
            cellA.SetPiece(cellB.GetPiece());
            cellB.SetPiece(temp);
        }
    }
}