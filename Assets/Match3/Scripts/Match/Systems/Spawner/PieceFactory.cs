using Match3.Scripts.Pieces;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Match3.Scripts.Match.Systems.Spawner
{
    public class PieceFactory
    {
        private readonly PiecesConfigurationSO _piecesConfigurationSO;

        public PieceFactory(PiecesConfigurationSO piecesConfigurationSo)
        {
            _piecesConfigurationSO = piecesConfigurationSo;
        }

        public Piece Create(string id, Vector2 position, Transform parent)
        {
            var pieceSo = _piecesConfigurationSO.GetPieceById(id);
            var piece = Object.Instantiate(pieceSo.prefab, position, Quaternion.identity, parent).GetComponent<Piece>();
            piece.Init(pieceSo);
            return piece;
        }
    }
}