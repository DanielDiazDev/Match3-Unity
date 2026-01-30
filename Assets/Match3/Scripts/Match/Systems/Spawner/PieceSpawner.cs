using Assets.Match3.Scripts.Grid;
using Match3.Scripts.Grid;
using Match3.Scripts.Pieces;
using UnityEngine;

namespace Match3.Scripts.Match.Systems.Spawner
{
    public class PieceSpawner
    {
        private readonly PieceFactory _piecesFactory;
        private readonly PiecesConfigurationSO _piecesConfigurationSO;
        private readonly GridSystem<GridObject> _gridSystem;

        public PieceSpawner(PiecesConfigurationSO piecesConfigurationSo, GridSystem<GridObject> gridSystem)
        {
            _piecesConfigurationSO = piecesConfigurationSo;
            _gridSystem = gridSystem;
            _piecesFactory = new PieceFactory(_piecesConfigurationSO); //Hacer inyeccion
        }

        public Piece Spawn(string id, Vector2Int position, Transform parent)
        {
            var piece = _piecesFactory.Create(id, _gridSystem.GetWorldPositionCenter(position.x, position.y), parent);
            var gridObject = new GridObject(_gridSystem, position.x, position.y);
            gridObject.SetPiece(piece);
            _gridSystem.SetValue(position.x, position.y, gridObject);
            return piece;
        }
    }
}