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

        public Block CreateBlock(string id, Vector2 position, Transform parent)
        {
            var blockSo = _piecesConfigurationSO.GetBlockById(id);
            var block = Object.Instantiate(blockSo.prefab, position, Quaternion.identity, parent).GetComponent<Block>();
            block.Init(blockSo);
            return block;
        }

        public Layer CreateLayer(string id, Vector2 position, Transform parent)
        {
            var layerSo = _piecesConfigurationSO.GetLayerById(id);
            var layer = Object.Instantiate(layerSo.prefab, position, Quaternion.identity, parent).GetComponent<Layer>();
            layer.Init(layerSo);
            return layer;
        }
    }
}