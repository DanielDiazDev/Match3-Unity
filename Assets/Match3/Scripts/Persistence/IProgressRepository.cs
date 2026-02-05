using System.Collections.Generic;
using Match3.Scripts.Data.Level;

namespace Match3.Scripts.Persistence
{
    public interface IProgressRepository
    {
        void Save(GameData gameData);
        GameData Load();
        bool Exists();
        void Delete();
        GameData CreateNewGame(LevelDataSO[] levelSOs);
        public LevelDataSO CurrentLevel { get; }


        void SetLevel(LevelDataSO levelData);
        void Clear();
    }
}