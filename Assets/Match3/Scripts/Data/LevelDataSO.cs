using System;
using System.Collections.Generic;
using AYellowpaper.SerializedCollections;
using Match3.Scripts.Data.Item;
using Match3.Scripts.Data.Quests;
using Match3.Scripts.Pieces;
using UnityEngine;

namespace Match3.Scripts.Data
{
    [CreateAssetMenu(fileName = "LevelDataSO", menuName = "Level/LevelDataSO")]
    public class LevelDataSO : ScriptableObject
    {
        public int id;
        public string levelName;
        public int width;
        public int height;
        [SerializedDictionary("Position", "Cell")]
        public SerializedDictionary<Vector2Int, TileSO> initialPieces = new();
        public List<TileSO> availableTiles = new();
        public List<QuestObjectiveSO> quests = new();
#if UNITY_EDITOR
        private void OnValidate()
        {
            if (width <= 0 || height <= 0) return;

            SyncDictionaryWithGrid();
        }

        private void SyncDictionaryWithGrid()
        {
            // Copia temporal para preservar valores
            var oldData = new Dictionary<Vector2Int, TileSO>(initialPieces);

            initialPieces.Clear();

            // ORDEN: X primero, luego Y
            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    var pos = new Vector2Int(x, y);

                    if (oldData.TryGetValue(pos, out var tile))
                        initialPieces.Add(pos, tile);
                    else
                        initialPieces.Add(pos, null);
                }
            }
        }
#endif



    }
}