using System;
using AYellowpaper.SerializedCollections;
using Match3.Scripts.Data.Item;
using UnityEngine;

namespace Match3.Scripts.Match.Systems.Spawner
{
    [CreateAssetMenu(fileName = "PiecesConfigurationSO", menuName = "Spawner/PiecesConfigurationSO")]
    public class PiecesConfigurationSO : ScriptableObject
    {
        [SerializedDictionary("id", "piece")] [SerializeField] private SerializedDictionary<string, PieceSO> _pieceDictionary;
        [SerializedDictionary("id", "block")] [SerializeField] private SerializedDictionary<string, BlockSO> _blockDictionary;
        [SerializedDictionary("id", "layer")] [SerializeField] private SerializedDictionary<string, LayerSO> _layerDictionary;

        public PieceSO GetPieceById(string id)
        {
            return !_pieceDictionary.TryGetValue(id, out var pieceSo) ? throw new Exception($"Piece with ID {id} not found") : pieceSo;
        }

        public BlockSO GetBlockById(string id)
        {
            return !_blockDictionary.TryGetValue(id, out var blockSo) ? throw new Exception($"Block with ID {id} not found") : blockSo;
        }

        public LayerSO GetLayerById(string id)
        {
            return !_layerDictionary.TryGetValue(id, out var layerSo) ? throw new Exception($"Layer with ID {id} not found") : layerSo;
        }
    }
}