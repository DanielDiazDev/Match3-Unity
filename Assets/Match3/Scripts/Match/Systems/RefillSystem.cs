using System.Collections.Generic;
using Assets.Match3.Scripts.Grid;
using Cysharp.Threading.Tasks;
using Match3.Scripts.Data;
using Match3.Scripts.Data.Item;
using Match3.Scripts.Grid;
using Match3.Scripts.Match.Systems.Spawner;
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

        public async UniTask ApplyRefill()
        {
            for (int x = 0; x < _gridSystem.Width; x++)
            {
                for (int y = 0; y < _gridSystem.Height; y++)
                {
                    var cell = _gridSystem.GetValue(x, y);
                    if (!cell.IsEmpty()) 
                        continue;

                    var validPieces = GetValidPiecesForPosition(x, y);

                    PieceSO selected = validPieces.Count > 0
                        ? validPieces[Random.Range(0, validPieces.Count)]
                        : _levelDataSO.availableTiles[Random.Range(0, _levelDataSO.availableTiles.Count)];
                    _pieceSpawner.Spawn(selected.id, new Vector2Int(x, y), _parent);
                }
            }

            await UniTask.Yield();
        }

        private List<PieceSO> GetValidPiecesForPosition(int x, int y)
        {
            var result = new List<PieceSO>();

            foreach (var piece in _levelDataSO.availableTiles)
            {
                if (!CreatesMatch(x, y, piece))
                    result.Add(piece);
            }

            return result;
        }

        private bool CreatesMatch(int x, int y, TileSO candidate)
        {
            // Horizontal ←←
            if (x >= 2)
            {
                var p1 = _gridSystem.GetValue(x - 1, y).GetPiece();
                var p2 = _gridSystem.GetValue(x - 2, y).GetPiece();

                if (p1 && p2 && p1.pieceSo == candidate && p2.pieceSo == candidate)
                    return true;
            }

            // Vertical ↓↓
            if (y >= 2)
            {
                var p1 = _gridSystem.GetValue(x, y - 1).GetPiece();
                var p2 = _gridSystem.GetValue(x, y - 2).GetPiece();

                if (p1 && p2 && p1.pieceSo == candidate && p2.pieceSo == candidate)
                    return true;
            }

            return false;
        }
      
    }
}