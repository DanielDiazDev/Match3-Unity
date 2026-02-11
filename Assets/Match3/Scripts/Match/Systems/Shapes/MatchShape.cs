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

            int maxH = 0;
            int maxV = 0;

            foreach (var pos in positions)
            {
                int h = 1;
                Vector2Int curr = pos + Vector2Int.left;
                while (positions.Contains(curr)) { h++; curr += Vector2Int.left; }
                curr = pos + Vector2Int.right;
                while (positions.Contains(curr)) { h++; curr += Vector2Int.right; }
                maxH = Mathf.Max(maxH, h);

                int v = 1;
                curr = pos + Vector2Int.down;
                while (positions.Contains(curr)) { v++; curr += Vector2Int.down; }
                curr = pos + Vector2Int.up;
                while (positions.Contains(curr)) { v++; curr += Vector2Int.up; }
                maxV = Mathf.Max(maxV, v);
            }

            if (maxH >= 3 && maxV >= 3)
            {
                // Likely T or L shape
                // Find potential center of the cross/T/L
                center = FindCrossCenter(positions);
                
                // Re-calculate arms from the found center
                int left = Count(center, positions, Vector2Int.left);
                int right = Count(center, positions, Vector2Int.right);
                int up = Count(center, positions, Vector2Int.up);
                int down = Count(center, positions, Vector2Int.down);

                int horizontalArms = (left > 0 ? 1 : 0) + (right > 0 ? 1 : 0);
                int verticalArms = (up > 0 ? 1 : 0) + (down > 0 ? 1 : 0);

                if (horizontalArms == 1 && verticalArms == 1)
                    return new ClassifiedMatch(match, MatchShape.LShape, center);

                if ((horizontalArms == 2 && verticalArms == 1) || (verticalArms == 2 && horizontalArms == 1))
                    return new ClassifiedMatch(match, MatchShape.TShape, center);
            }

            if (maxH >= 5 || maxV >= 5)
                return new ClassifiedMatch(match, MatchShape.Line5, center);

            if (maxH == 4 || maxV == 4)
                return new ClassifiedMatch(match, MatchShape.Line4, center);

            return new ClassifiedMatch(match, MatchShape.Line3, center);
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
