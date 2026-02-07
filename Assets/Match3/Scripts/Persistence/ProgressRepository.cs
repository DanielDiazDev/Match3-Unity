using System;
using System.Collections.Generic;
using AYellowpaper.SerializedCollections;
using Match3.Scripts.Data.Level;

namespace Match3.Scripts.Persistence
{
    public class ProgressRepository : IProgressRepository
    {
        private const string FILE_NAME = "gameData";
        public LevelDataSO CurrentLevel { get; private set; }
        
        public GameData CreateNewGame(LevelDataSO[] levelSOs)
        {
            GameData data = new GameData
            {
                coins = 0,
                hearts = 5,
                maxUnlockedLevel = 0,
                levels = new LevelProgress[levelSOs.Length],
                lastHeartUpdateTick = DateTime.Now.Ticks
            };

            for (int i = 0; i < levelSOs.Length; i++)
            {
                data.levels[i] = new LevelProgress
                {
                    id = levelSOs[i].id,
                    stars = 0,
                    completed = false,
                    unlocked = i == 0 
                };
            }
            
            data.purchasedPowerUps = new SerializedDictionary<string, int>();

            Save(data);
            return data;
        }


        public void Save(GameData gameData)
        {
            SaveSystem.Save(gameData, FILE_NAME, false);
        }

        public GameData Load()
        {
            GameData data;
            if (!Exists())
                data = CreateDefault();
            else
                data = SaveSystem.Load<GameData>(FILE_NAME, true);

            if (data != null)
            {
                ApplyHeartRegeneration(data);
            }

            return data;
        }

        private void ApplyHeartRegeneration(GameData data)
        {
            const int MAX_HEARTS = 5;
            const int MINUTES_PER_HEART = 30;
            TimeSpan regenerationInterval = TimeSpan.FromMinutes(MINUTES_PER_HEART);

            if (data.hearts >= MAX_HEARTS)
            {
                data.lastHeartUpdateTick = DateTime.Now.Ticks;
                return;
            }

            DateTime lastUpdateTime = new DateTime(data.lastHeartUpdateTick);
            DateTime now = DateTime.Now;
            TimeSpan elapsed = now - lastUpdateTime;

            if (elapsed >= regenerationInterval)
            {
                int heartsToAdd = (int)(elapsed.TotalMinutes / MINUTES_PER_HEART);
                int newHearts = Math.Min(MAX_HEARTS, data.hearts + heartsToAdd);

                if (newHearts > data.hearts)
                {
                    data.hearts = newHearts;
                    // Update ticks to the time of the last added heart to avoid losing partial progress
                    long usedTicks = heartsToAdd * regenerationInterval.Ticks;
                    data.lastHeartUpdateTick = lastUpdateTime.Ticks + usedTicks;
                    Save(data);
                }
            }
        }

        public void UpdateLevelProgress(int levelId, int stars, bool completed)
        {
            var data = Load();
            for (int i = 0; i < data.levels.Length; i++)
            {
                if (data.levels[i].id == levelId)
                {
                    data.levels[i].stars = Math.Max(data.levels[i].stars, stars);
                    data.levels[i].completed = data.levels[i].completed || completed;

                    if (completed && i + 1 < data.levels.Length)
                    {
                        data.levels[i + 1].unlocked = true;
                    }
                    break;
                }
            }
            Save(data);
        }

        public void SavePlayerData(int coins, int hearts)
        {
            var data = Load(); // Load applies regeneration first
            data.coins = coins;
            data.hearts = hearts;
            Save(data);
        }

        public IReadOnlyDictionary<string, int> GetPurchasedPowerUps()
        {
            var data = Load();
            return data.purchasedPowerUps;
        }

        public void ConsumePowerUp(string id)
        {
            var data = Load();
            if (data.purchasedPowerUps.ContainsKey(id))
            {
                data.purchasedPowerUps[id]--;
                if (data.purchasedPowerUps[id] <= 0)
                {
                    data.purchasedPowerUps.Remove(id);
                }
                Save(data);
            }
        }

        public bool Exists()
        {
            return SaveSystem.Exists(FILE_NAME);
        }

        public void Delete()
        {
            SaveSystem.Delete(FILE_NAME);
        }

        
        public void SetLevel(LevelDataSO levelData)
        {
            CurrentLevel = levelData;
        }

        public void Clear()
        {
            CurrentLevel = null;
        }
        private GameData CreateDefault()
        {
            return new GameData
            {
                coins = 0,
                hearts = 5,
                maxUnlockedLevel = 0,
                currentLevel = 1,
                lastHeartUpdateTick = DateTime.Now.Ticks,
                purchasedPowerUps = new SerializedDictionary<string, int>()
            };
        }

        public void AddPowerUp(string id, int amount)
        {
            var gameData = Load();
            if (gameData.purchasedPowerUps.ContainsKey(id))
            {
                gameData.purchasedPowerUps[id] += amount;
            }
            else
            {
                gameData.purchasedPowerUps.Add(id, amount);
            }
            Save(gameData);
        }

        public int GetPowerUpCount(string id)
        {
            var gameData = Load();
            if (gameData.purchasedPowerUps != null && gameData.purchasedPowerUps.ContainsKey(id))
            {
                return gameData.purchasedPowerUps[id];
            }
            return 0;
        }
    }
}