using System.Collections.Generic;
using System.Threading.Tasks;
using Match3.Scripts.Data.Level;
using UnityEngine;
using UnityEngine.UIElements;

namespace Match3.Scripts.Level
{
    public class LevelModel
    {
        public int id;
        public int stars;
        public bool unlocked;
        public LevelDataSO levelData;
        public LevelModel(int id, int stars, bool unlocked, LevelDataSO levelData)
        {
            this.id = id;
            this.stars = stars;
            this.unlocked = unlocked;
            this.levelData = levelData;
        }
    }
}
