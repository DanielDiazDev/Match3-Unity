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
                levels = new LevelProgress[levelSOs.Length]
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

            Save(data);
            return data;
        }


        public void Save(GameData gameData)
        {
            SaveSystem.Save(gameData, FILE_NAME, false);
        }

        public GameData Load()
        {
            if (!Exists())
                return CreateDefault();

            return SaveSystem.Load<GameData>(FILE_NAME, true);
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
                currentLevel = 1
            };
        }
    }
}