using System.Collections.Generic;
using AYellowpaper.SerializedCollections;
using Match3.Scripts.Data.Item;
using Match3.Scripts.Data.Quests;
using UnityEngine;

namespace Match3.Scripts.Data.Level
{
    [CreateAssetMenu(fileName = "LevelDataSO", menuName = "Level/LevelDataSO")]
    public class LevelDataSO : ScriptableObject
    {
        public int id;
        public string levelName;
        public int width;
        public int height;
        [SerializedDictionary("Position", "Data")]
        public SerializedDictionary<Vector2Int, LevelCellData> initialGridData = new();
        public List<TileSO> availableTiles = new();
        public List<QuestObjectiveSO> quests = new();
        public int maxMoves;
        [Range(1,5)]
        public int difficulty;

        public int[] starThresholds = new int[3] { 1000, 2000, 3000 };
        public int objectiveBonus = 500;
        
        public Sprite background;
    }

    [System.Serializable]
    public class LevelCellData
    {
        public TileSO tile;
        public BlockSO block;
        public LayerSO layer;
    }
}