using System.Linq;
using Match3.Scripts.Data.Level;
using Match3.Scripts.Level;
using Match3.Scripts.Persistence;

public class LevelUseCase : ILevelUseCase
{
    private IProgressRepository _progressRepository;
    private ILoadScene _loadScene;
    private LevelDataSO[] _allLevels;

    public LevelUseCase(IProgressRepository progressRepository,
        ILoadScene loadScene,
        LevelDataSO[] allLevels)
    {
        _progressRepository = progressRepository;
        _loadScene = loadScene;
        _allLevels = allLevels;
    }

    public LevelModel BuildLevel(int id)
    {
        var gameData = _progressRepository.Load();
        var progress = gameData.levels.FirstOrDefault(l => l.id == id);
        var levelData = _allLevels.FirstOrDefault(l => l.id == id);

        return new LevelModel(id, progress.stars, progress.unlocked, levelData);
    }

    public void SelectLevel(LevelDataSO levelData)
    {
        var gameData = _progressRepository.Load();
        var progress = gameData.levels.FirstOrDefault(l => l.id == levelData.id);

        if (!progress.unlocked)
            return;

        gameData.currentLevel = levelData.id;
        _progressRepository.Save(gameData);

        _progressRepository.SetLevel(levelData);
        _loadScene.Load();
    }
}


// using System.Linq;
// using Match3.Scripts.Data.Level;
// using Match3.Scripts.Persistence;
// using Match3.Scripts.Utils;
//
// namespace Match3.Scripts.Level
// {
//     public class LevelUseCase : ILevelUseCase
//     {
//         private IProgressRepository _progressRepository;
//         private ILoadScene _loadScene;
//         public LevelUseCase(IProgressRepository progressRepository, ILoadScene loadScene)
//         {
//             _progressRepository = progressRepository;
//             _loadScene = loadScene;
//         }
//         public LevelModel BuildLevel(int id)
//         {
//             var gameData = _progressRepository.Load();
//             var level = gameData.levels.FirstOrDefault(l => l.id == id);
//             return new LevelModel(id, level.stars, level.unlocked, _levelData);
//         }
//
//         public void SelectLevel(LevelDataSO levelData)
//         {
//             var gameData = _progressRepository.Load();
//             var level = gameData.levels.FirstOrDefault(l => l.id == levelData.id);
//
//             if (!level.unlocked) 
//                 return;
//
//             gameData.currentLevel = levelData.id;
//             _progressRepository.Save(gameData);
//
//             _progressRepository.SetLevel(levelData);
//             _loadScene.Load();
//         }
//     }
// }