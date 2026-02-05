using System;

namespace Match3.Scripts.Persistence
{
    [Serializable]
    public class GameData
    {
        public int coins;
        public int hearts;
        public int maxUnlockedLevel;
        public int currentLevel;
        public LevelProgress[] levels;
    }
    [Serializable]
    public class LevelProgress
    {
        public int id;
        public int stars;
        public bool unlocked;
        public bool completed;
    }
}