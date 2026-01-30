using System;
using System.Collections.Generic;
using System.Linq;
using Assets.Match3.Scripts.Grid;
using Match3.Scripts.Data.Item;
using Match3.Scripts.Grid;
using UnityEngine;

namespace Match3.Scripts.Match.Systems.Find
{
    public class MatchFinder
    {
        private readonly GridSystem<GridObject> _gridSystem;
        private readonly int _width;
        private readonly int _height;

        public MatchFinder(GridSystem<GridObject> gridSystem, int width, int height)
        {
            _gridSystem = gridSystem;
            _width = width;
            _height = height;
        }

        public List<PieceMatch> FindMatches(List<Vector2Int> positionsAffected)
        {
            HashSet<Vector2Int> globalVisited = new HashSet<Vector2Int>();
            List<PieceMatch> results = new List<PieceMatch>();

            foreach (var position in positionsAffected)
            {
                if (globalVisited.Contains(position))
                    continue;

                var cluster = GetConnectedCluster(position);
                if (cluster.Count < 3)
                    continue;

                if (!HasValidLine(cluster))
                    continue;

                PieceSO pieceSo = _gridSystem
                    .GetValue(position.x, position.y)
                    .GetPiece().pieceSo;

                results.Add(new PieceMatch(pieceSo, cluster));

                foreach (var pos in cluster)
                    globalVisited.Add(pos);
            }

            return results;
        }

        // 🔹 Flood Fill (cluster conectado)
        private List<Vector2Int> GetConnectedCluster(Vector2Int start)
        {
            List<Vector2Int> result = new List<Vector2Int>();
            Queue<Vector2Int> queue = new Queue<Vector2Int>();
            HashSet<Vector2Int> visited = new HashSet<Vector2Int>();

            PieceSO pieceSo = _gridSystem
                .GetValue(start.x, start.y)
                .GetPiece().pieceSo;

            queue.Enqueue(start);
            visited.Add(start);

            while (queue.Count > 0)
            {
                var current = queue.Dequeue();
                result.Add(current);

                foreach (var dir in Directions)
                {
                    var next = current + dir;

                    if (!_gridSystem.IsValid(next.x, next.y))
                        continue;

                    if (visited.Contains(next))
                        continue;
                    var piece = _gridSystem.GetValue(next.x, next.y).GetPiece();
                    if(piece == null) continue;
                    if (piece.pieceSo != pieceSo) continue;
                    visited.Add(next);
                    queue.Enqueue(next);


                }
            }

            return result;
        }

        // 🔹 Validación real de match (líneas de 3)
        private bool HasValidLine(List<Vector2Int> cluster)
        {
            foreach (var pos in cluster)
            {
                int horizontal = cluster.Count(p => p.y == pos.y);
                int vertical = cluster.Count(p => p.x == pos.x);

                if (horizontal >= 3 || vertical >= 3)
                    return true;
            }

            return false;
        }

        private static readonly Vector2Int[] Directions =
        {
            Vector2Int.up,
            Vector2Int.down,
            Vector2Int.left,
            Vector2Int.right
        };
    }
}
