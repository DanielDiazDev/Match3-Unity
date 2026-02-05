using System;
using System.Collections.Generic;
using Match3.Scripts.Data.Level;
using UniRx;

namespace Match3.Scripts.Level
{
    public class LevelController : IDisposable
    {
        private LevelViewModel _levelViewModel;
        private LevelDetailViewModel _levelDetailViewModel;
        private ILevelUseCase _levelUseCase; 
        private List<IDisposable> _disposables = new List<IDisposable>();
        private LevelModel _currentSelectedLevel;
        public LevelController(LevelViewModel levelViewModel, ILevelUseCase levelUseCase, LevelDetailViewModel levelDetailViewModel, LevelDataSO levelDataSo)
        {
            _levelViewModel = levelViewModel;
            _levelUseCase = levelUseCase;
            _levelDetailViewModel = levelDetailViewModel;

            _levelViewModel.LevelButtonPressed.Subscribe(OnLevelButtonPressed).AddTo(_disposables);
            _levelDetailViewModel.StartLevel.Subscribe(OnStartPlayButtonPressed).AddTo(_disposables);
        }
        
        private void OnLevelButtonPressed(string levelId)
        {
            var level = _levelUseCase.BuildLevel(int.Parse(levelId));

            _currentSelectedLevel = level;

            _levelDetailViewModel.LevelId.Value = level.id.ToString();
            _levelDetailViewModel.Stars.Value = level.stars;
        }


        private void OnStartPlayButtonPressed(Unit _)
        {
            if (_currentSelectedLevel == null)
                return;

            _levelUseCase.SelectLevel(_currentSelectedLevel.levelData);
        }

        public void Dispose()
        {
            foreach (var disposable in _disposables)
            {
                disposable.Dispose();
            }
        }
    }
}