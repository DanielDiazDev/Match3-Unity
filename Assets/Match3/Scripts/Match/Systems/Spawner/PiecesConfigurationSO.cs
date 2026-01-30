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

        public PieceSO GetPieceById(string id)
        {
            return !_pieceDictionary.TryGetValue(id, out var pieceSo) ? throw new Exception($"Piece with ID {id} not found") : pieceSo;
        }
    }
}