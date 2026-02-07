using System;
using System.Collections.Generic;
using Match3.Scripts.Data.Level;
using Match3.Scripts.Persistence;
using Match3.Scripts.ServiceLocator;
using Match3.Scripts.Shop;
using UnityEngine;
using UnityEngine.UI;

namespace Match3.Scripts.Level
{
    public class LevelManager : MonoBehaviour
    {
        public static LevelManager Instance { get; private set; }

        [SerializeField] private LevelDataSO[] _levelDataSos;
        [SerializeField] private GameObject _levelPrefab;
        [SerializeField] private Transform _levelParent;
        private readonly IProgressRepository _progressRepository =  new ProgressRepository();
        private readonly LoadScene _loadScene = new LoadScene();
        private readonly List<LevelController> _controllers = new List<LevelController>();
        [SerializeField] private LevelDetailView _levelDetailView;
        [SerializeField] private CanvasGroup _levelSelectorCanvasGroup;
        [SerializeField] private Button _openShopButton;
        private readonly LevelDetailViewModel _levelDetailViewModel = new LevelDetailViewModel();

        private void Awake()
        {
            ServiceLocator.ServiceLocator.Global.Register(_progressRepository);
            if(Instance == null)
            {
                Instance = this;
            }
            else
            {
                Destroy(gameObject);
            }
        }
        

        private void Start()
        {
            GameData gameData;
            if (!_progressRepository.Exists())
            {
                gameData = _progressRepository.CreateNewGame(_levelDataSos);
            }
            else
            {
                gameData = _progressRepository.Load();
            }

            _openShopButton.onClick.AddListener(OpenShop);

            GenerateLevels();
        }

        private void OpenShop()
        {
            SetLevelSelectorInteractable(false);
            var shopManager = FindFirstObjectByType<ShopManager>();
            if (shopManager != null)
            {
                shopManager.OpenShop();
            }
        }

        public void SetLevelSelectorInteractable(bool interactable)
        {
            _openShopButton.gameObject.SetActive(interactable);
            _levelSelectorCanvasGroup.interactable = interactable;
            _levelSelectorCanvasGroup.blocksRaycasts = interactable;
        }

        private void GenerateLevels()
        {
            var levelUseCase = new LevelUseCase(_progressRepository, _loadScene, _levelDataSos);
            foreach (var levelDataSo in _levelDataSos)
            {
                var levelViewModel = new LevelViewModel();
                
                var levelPresenter = new LevelPresenter(levelViewModel);
                var level = levelUseCase.BuildLevel(levelDataSo.id);
                var levelController = new LevelController(levelViewModel, levelUseCase, _levelDetailViewModel, levelDataSo);
                _controllers.Add(levelController);
                levelPresenter.Present(level);
                var view = Instantiate(_levelPrefab, _levelParent).GetComponent<LevelView>();
                view.Configure(levelViewModel);
                _levelDetailView.Configure(_levelDetailViewModel);
            }
        }

        public void ShowLevelDetail()
        {
            _levelDetailView.Show();
        }

        public void HideLevelDetail()
        {
            _levelDetailView.Hide();
        }
        private void OnDestroy()
        {
            foreach (var controller in _controllers)
            {
                controller.Dispose();
            }
        }
    }
}