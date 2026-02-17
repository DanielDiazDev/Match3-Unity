using System.Collections.Generic;
using System.Linq;
using Assets.Match3.Scripts.Grid;
using Cysharp.Threading.Tasks;
using Match3.Scripts.Data.Item;
using Match3.Scripts.Data.Level;
using Match3.Scripts.Grid;
using Match3.Scripts.Match.Systems.Spawner;
using PrimeTween;
using UnityEngine;

namespace Match3.Scripts.Match.Systems
{
    public class RefillSystem
    {
        private LevelDataSO _levelDataSO;
        private GridSystem<GridObject> _gridSystem;
        private PieceSpawner _pieceSpawner;
        private Transform _parent;
        public RefillSystem(LevelDataSO levelDataSo, GridSystem<GridObject> gridSystem, PieceSpawner pieceSpawner, Transform parent)
        {
            _levelDataSO = levelDataSo;
            _gridSystem = gridSystem;
            _pieceSpawner = pieceSpawner;
            _parent = parent;
        }

        public async UniTask ApplyRefill(int difficulty = 1, int cascadeCount = 0)
        {
            Sequence sequence = default;
            bool sequenceCreated = false;

            for (int x = 0; x < _gridSystem.Width; x++)
            {
                int missingCount = 0;

                for (int y = _gridSystem.Height - 1; y >= 0; y--)
                {
                    var cell = _gridSystem.GetValue(x, y);

                    if (!cell.IsPlayable() || cell.HasBlock())
                        break;

                    if (!cell.IsEmpty())
                        continue;

                    missingCount++;

                    TileSO selected = SelectPieceWithDifficulty(x, y, difficulty, cascadeCount);

                    var piece = _pieceSpawner.Spawn(selected.id, new Vector2Int(x, y), _parent);

                    var targetPos = _gridSystem.GetWorldPositionCenter(x, y);
                    var startPos = targetPos + Vector3.up * (_gridSystem.Height - y + missingCount);

                    piece.transform.position = startPos;

                    var tween = Tween.Position(
                        piece.transform,
                        targetPos,
                        0.4f,
                        Ease.OutBounce);

                    if (!sequenceCreated)
                    {
                        sequence = Sequence.Create();
                        sequenceCreated = true;
                    }

                    sequence.Group(tween);

                    var spawnTween = piece.PlaySpawnAnimation();
                    sequence.Group(spawnTween);
                }
            }

            if (sequenceCreated)
            {
                await sequence; 
            }
        }

        private TileSO SelectPieceWithDifficulty(int x, int y, int difficulty, int cascadeCount)
        {
            var available = _levelDataSO.availableTiles;
            
            bool avoidNearMatches = difficulty >= 4 && cascadeCount > 0;

            var candidates = available.Where(p => !CreatesMatch(x, y, p)).ToList();

            if (avoidNearMatches && candidates.Count > 1)
            {
                var verySafeCandidates = candidates.Where(p => !CreatesNearMatch(x, y, p)).ToList();
                if (verySafeCandidates.Count > 0)
                    return verySafeCandidates[Random.Range(0, verySafeCandidates.Count)];
            }

            if (candidates.Count > 0)
                return candidates[Random.Range(0, candidates.Count)];

            return available[Random.Range(0, available.Count)];
        }

        private bool CreatesMatch(int x, int y, TileSO candidate)
        {
            // Horizontal check (Left)
            if (x >= 2)
            {
                var p1 = _gridSystem.GetValue(x - 1, y)?.GetPiece();
                var p2 = _gridSystem.GetValue(x - 2, y)?.GetPiece();
                if (p1 && p1.pieceSo == candidate && p2 && p2.pieceSo == candidate) return true;
            }
            // Horizontal check (Right)
            if (x < _gridSystem.Width - 2)
            {
                var p1 = _gridSystem.GetValue(x + 1, y)?.GetPiece();
                var p2 = _gridSystem.GetValue(x + 2, y)?.GetPiece();
                if (p1 && p1.pieceSo == candidate && p2 && p2.pieceSo == candidate) return true;
            }
            // Horizontal check (Middle)
            if (x >= 1 && x < _gridSystem.Width - 1)
            {
                var p1 = _gridSystem.GetValue(x - 1, y)?.GetPiece();
                var p2 = _gridSystem.GetValue(x + 1, y)?.GetPiece();
                if (p1 && p1.pieceSo == candidate && p2 && p2.pieceSo == candidate) return true;
            }

            // Vertical check (Down)
            if (y >= 2)
            {
                var p1 = _gridSystem.GetValue(x, y - 1)?.GetPiece();
                var p2 = _gridSystem.GetValue(x, y - 2)?.GetPiece();
                if (p1 && p1.pieceSo == candidate && p2 && p2.pieceSo == candidate) return true;
            }
            // Vertical check (Up)
            if (y < _gridSystem.Height - 2)
            {
                var p1 = _gridSystem.GetValue(x, y + 1)?.GetPiece();
                var p2 = _gridSystem.GetValue(x, y + 2)?.GetPiece();
                if (p1 && p1.pieceSo == candidate && p2 && p2.pieceSo == candidate) return true;
            }
            // Vertical check (Middle)
            if (y >= 1 && y < _gridSystem.Height - 1)
            {
                var p1 = _gridSystem.GetValue(x, y - 1)?.GetPiece();
                var p2 = _gridSystem.GetValue(x, y + 1)?.GetPiece();
                if (p1 && p1.pieceSo == candidate && p2 && p2.pieceSo == candidate) return true;
            }

            return false;
        }

        private bool CreatesNearMatch(int x, int y, TileSO candidate)
        {
            // Check if any neighbor is the same color
            if (x > 0 && _gridSystem.GetValue(x - 1, y)?.GetPiece()?.pieceSo == candidate) return true;
            if (x < _gridSystem.Width - 1 && _gridSystem.GetValue(x + 1, y)?.GetPiece()?.pieceSo == candidate) return true;
            if (y > 0 && _gridSystem.GetValue(x, y - 1)?.GetPiece()?.pieceSo == candidate) return true;
            if (y < _gridSystem.Height - 1 && _gridSystem.GetValue(x, y + 1)?.GetPiece()?.pieceSo == candidate) return true;
            
            return false;
        }
      
    }
}