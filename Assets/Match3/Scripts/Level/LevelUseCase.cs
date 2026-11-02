using System.Linq;
using Match3.Scripts.Data.Level;
using Match3.Scripts.Level;
using Match3.Scripts.Persistence;
using Match3.Scripts.ServiceLocator;
using Match3.Scripts.Sound;

public class LevelUseCase : ILevelUseCase
{
    private IProgressRepository _progressRepository;
    private ILoadScene _loadScene;
    private LevelDataSO[] _allLevels;
    private bool _isSelectingLevel; // Evita llamadas múltiples (doble clic)

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
        // Evitar llamadas múltiples (doble clic en el botón de start)
        if (_isSelectingLevel)
            return;

        var gameData = _progressRepository.Load();
        var progress = gameData.levels.FirstOrDefault(l => l.id == levelData.id);

        if (!progress.unlocked)
            return;

        if (gameData.hearts <= 0)
        {
            UnityEngine.Debug.LogWarning("Not enough hearts to start level!");
            return;
        }

        // Si ya estamos en este nivel, no descontar otro corazón
        if (gameData.currentLevel == levelData.id)
        {
            _progressRepository.SetLevel(levelData);
            ServiceLocator.Global.Get<SoundManager>().PlaySFX(SoundId.Select);
            _loadScene.Load();
            return;
        }

        _isSelectingLevel = true;

        gameData.hearts--;
        gameData.currentLevel = levelData.id;
        _progressRepository.Save(gameData);

        _progressRepository.SetLevel(levelData);
        ServiceLocator.Global.Get<SoundManager>().PlaySFX(SoundId.Select);
        _loadScene.Load();

        _isSelectingLevel = false;
    }
}


