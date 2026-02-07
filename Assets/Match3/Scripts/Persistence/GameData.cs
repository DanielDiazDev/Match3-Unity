using System;
using System.Collections.Generic;
using AYellowpaper.SerializedCollections;

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
        public long lastHeartUpdateTick;
        public SerializedDictionary<string, int> purchasedPowerUps;
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