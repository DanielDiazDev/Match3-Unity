using System.Collections.Generic;
using System.Linq;
using Match3.Scripts.Data.Item;
using Match3.Scripts.Match.Systems.Find;
using UnityEngine;

namespace Match3.Scripts.Match.Systems.Shapes
{
    public enum MatchShape
    {
        Line3,
        Line4,
        Line5,
        TShape,
        LShape
    }

    public class ClassifiedMatch
    {
        public PieceSO pieceSo;
        public List<Vector2Int> positions;
        public MatchShape shape;
        public Vector2Int center;

        public ClassifiedMatch(PieceMatch match, MatchShape shape, Vector2Int center)
        {
            pieceSo = match.pieceSo;
            positions = match.positions;
            this.shape = shape;
            this.center = center;
        }
    }

    public class MatchClassifier
    {
        public ClassifiedMatch Classify(PieceMatch match)
        {
            var positions = match.positions;
            Vector2Int center = FindCrossCenter(positions);

            bool hasHorizontal = HasLine(positions, Vector2Int.right);
            bool hasVertical = HasLine(positions, Vector2Int.up);
            int left  = Count(center, positions, Vector2Int.left);
            int right = Count(center, positions, Vector2Int.right);
            int up    = Count(center, positions, Vector2Int.up);
            int down  = Count(center, positions, Vector2Int.down);

            int horizontal = left + right + 1;
            int vertical   = up + down + 1;

            if (horizontal >= 3 && vertical >= 3)
            {
                bool hasLeft  = left > 0;
                bool hasRight = right > 0;
                bool hasUp    = up > 0;
                bool hasDown  = down > 0;

                int horizontalArms = (hasLeft ? 1 : 0) + (hasRight ? 1 : 0);
                int verticalArms   = (hasUp ? 1 : 0) + (hasDown ? 1 : 0);

                if (horizontalArms == 1 && verticalArms == 1)
                {
                    return new ClassifiedMatch(match, MatchShape.LShape, center);
                }

                if ((horizontalArms == 2 && verticalArms == 1) ||
                    (verticalArms == 2 && horizontalArms == 1))
                {
                    return new ClassifiedMatch(match, MatchShape.TShape, center);
                }
            }


            if (positions.Count == 5)
                return new ClassifiedMatch(match, MatchShape.Line5, center);

            if (positions.Count == 4)
                return new ClassifiedMatch(match, MatchShape.Line4, center);

            return new ClassifiedMatch(match, MatchShape.Line3, center);
        }

        private bool HasLine(List<Vector2Int> positions, Vector2Int dir)
        {
            foreach (var p in positions)
            {
                if (positions.Contains(p + dir) &&
                    positions.Contains(p - dir))
                    return true;
            }
            return false;
        }

        private int Count(Vector2Int center, List<Vector2Int> positions, Vector2Int dir)
        {
            int count = 0;
            Vector2Int current = center + dir;

            while (positions.Contains(current))
            {
                count++;
                current += dir;
            }

            return count;
        }
        private Vector2Int FindCrossCenter(List<Vector2Int> positions)
        {
            foreach (var pos in positions)
            {
                bool hasHorizontal =
                    positions.Contains(pos + Vector2Int.left) ||
                    positions.Contains(pos + Vector2Int.right);

                bool hasVertical =
                    positions.Contains(pos + Vector2Int.up) ||
                    positions.Contains(pos + Vector2Int.down);

                if (hasHorizontal && hasVertical)
                    return pos;
            }

            return GetFallbackCenter(positions);
        }

        private Vector2Int GetFallbackCenter(List<Vector2Int> positions)
        {
            int x = Mathf.RoundToInt(positions.Average(p => (float)p.x));
            int y = Mathf.RoundToInt(positions.Average(p => (float)p.y));
            return new Vector2Int(x, y);
        }
    }
}
