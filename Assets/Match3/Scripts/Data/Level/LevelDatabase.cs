using UnityEngine;

namespace Match3.Scripts.Data.Level
{
    [CreateAssetMenu(fileName = "LevelDatabase", menuName = "Level/LevelDatabase")]
    public class LevelDatabase : ScriptableObject
    {
        public LevelDataSO[] levels;
    }
}