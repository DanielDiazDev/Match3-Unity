using Assets.Match3.Scripts.Grid;
using Cysharp.Threading.Tasks;
using Match3.Scripts.Data.Item;
using Match3.Scripts.Grid;
using System;
using System.Collections.Generic;
using System.Linq;
using Match3.Scripts.Data.Level;
using Match3.Scripts.Event;
using Match3.Scripts.Event.Data;
using Match3.Scripts.Match.Systems;
using Match3.Scripts.Match.Systems.Find;
using Match3.Scripts.Match.Systems.Spawner;
using Match3.Scripts.Persistence;
using Match3.Scripts.PowerUps;
using Match3.Scripts.Quests;
using Match3.Scripts.Score;
using Match3.Scripts.UI.Inventory;
using Reflex.Attributes;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Match3.Scripts.Core
{
    public class MatchManager : MonoBehaviour, IEventObserver
    {
        [SerializeField] private float _cellSize = 1f;
        [SerializeField] private bool _debug = true;
        [SerializeField] private LevelDataSO _levelDataSoTest;
        [SerializeField] private PowerUpSystem  _powerUpSystem; //No va
        [SerializeField] private PiecesConfigurationSO  _piecesConfigurationSO; //No va
        private IScoreSystem _scoreSystem;
        private IQuestSystem _questSystem;//No va
        [SerializeField] private GridObjectDebugView _selectedCellDebug;
        [SerializeField] private GameObject _backgroundTilePrefab;
        private StarSystem _starSystem;
        private Vector3 _origin; 
        private int _remainingMoves;//No va
        private LevelDataSO _levelDataSO;
        private GridSystem<GridObject>  _gridSystem;
        public bool IsBusy => _isBusy;
        private bool _isBusy;
        private GridObject _selectedCell;
        //TODO: Agregarle interface a los sistemas de gameplay
        private PieceSwapper _pieceSwapper;
        private RefillSystem _refillSystem;
        private PieceSpawner _pieceSpawner;
        private MatchFinder _matchFinder;
        private MatchProcessor _matchProcessor;
        private GravitySystem _gravitySystem;
        private MoveValidator _moveValidator;
        private ShuffleBoardSystem _shuffleBoardSystem;
        private IProgressRepository _progressRepository;
        private TurnFlowController _turnFlowController;
        private BoardResolver _boardResolver;
        private EndGameSequenceController _endGameController;
        private bool _gameEnded;
        
        private void OnDestroy()
        {
            EventQueue.Instance.Unsubscribe(EventIds.SwipeDetected, this);
        }
        [Inject]
        public void Construct(IScoreSystem scoreSystem)
        {
            _scoreSystem = scoreSystem;
        }
        public void Process(EventData eventData)
        {
            if(eventData.EventId != EventIds.SwipeDetected) return;
            var swipeDetectedEventData = (SwipeDetectedEventData)eventData;
            OnSwipeItem(swipeDetectedEventData.worldStart, swipeDetectedEventData.direction);
        }

        private void Start()
        {
            EventQueue.Instance.Subscribe(EventIds.SwipeDetected, this);
            InitializeSystems();
            InitializeGrid();
        }
        private void Update()
        {
            //if (!_debug) return;

            if (Touchscreen.current != null && Touchscreen.current.press.wasPressedThisFrame)
            {
                HandleClick();
            }
        }
        private void InitializeSystems()
        {
            if (_debug)
            {
                _levelDataSO = _levelDataSoTest;
            }
            else
            {
                _levelDataSO = ServiceLocator.ServiceLocator.Global.Get<IProgressRepository>().CurrentLevel;
            }

            _progressRepository = new ProgressRepository();
            _origin = new Vector3(-_levelDataSO.width * _cellSize * 0.5f, -_levelDataSO.height * _cellSize * 0.5f, 0f);
            _gridSystem = GridSystem<GridObject>.VerticalGrid(_levelDataSO.width, _levelDataSO.height, _cellSize, _origin, _debug);
            _pieceSpawner = new PieceSpawner(_piecesConfigurationSO, _gridSystem);
            _pieceSwapper = new PieceSwapper(_gridSystem);
            _matchFinder = new MatchFinder(_gridSystem, _levelDataSO.width, _levelDataSO.height);
            _questSystem = new QuestSystem(_levelDataSO.quests);
            _starSystem = new StarSystem(_levelDataSO, _questSystem, _scoreSystem);

            _remainingMoves = _levelDataSO.maxMoves;
            EventQueue.Instance.EnqueueEvent(new MovesChangedEventData(EventIds.MovesChanged, _remainingMoves));
            _powerUpSystem.Init(_gridSystem, _scoreSystem, _questSystem);
            _matchProcessor = new MatchProcessor(_gridSystem, _powerUpSystem, _scoreSystem);
            _gravitySystem = new GravitySystem(_gridSystem, _levelDataSO.width, _levelDataSO.height);
            _refillSystem = new RefillSystem(_levelDataSO, _gridSystem, _pieceSpawner, transform);
            _moveValidator = new MoveValidator(_gridSystem, _matchFinder);
            _shuffleBoardSystem = new ShuffleBoardSystem(_gridSystem, _matchFinder, _moveValidator);
            _boardResolver =
                new BoardResolver(_matchFinder, _matchProcessor, _gravitySystem, _refillSystem, _gridSystem);
            _turnFlowController = new TurnFlowController(_pieceSwapper, _moveValidator, _boardResolver, _powerUpSystem,
                _scoreSystem, _gridSystem, _matchFinder);
            _endGameController = new EndGameSequenceController(_gridSystem, _powerUpSystem, _boardResolver);
        }
        private void SetBusy(bool busy)
        {
            _isBusy = busy;
            // if (_inventoryViewModel != null)
            // {
            //     _inventoryViewModel.IsBusy.Value = busy;
            // }
        }
        //No va aca, installer o manager
       
        private void InitializeGrid()
        {
            for (var x = 0; x < _levelDataSO.width; x++) 
            {
                for (var y = 0; y < _levelDataSO.height; y++)
                {
                    var position = new Vector2Int(x, y);
                    
                    if (!_levelDataSO.initialGridData.TryGetValue(position, out var cellData))
                    {
                        var gridObject = new GridObject(_gridSystem, x, y);
                        gridObject.SetPlayable(false);
                        _gridSystem.SetValue(x, y, gridObject);
                        continue;
                    }
                    SpawnBackground(position);
                    if (cellData.block != null)
                    {
                        _pieceSpawner.SpawnBlock(cellData.block.id, position, transform);
                    }
                    else if (cellData.tile != null)
                    {
                        var piece = _pieceSpawner.Spawn(cellData.tile.id, position, transform);
                       // piece.PlaySpawnAnimation();
                    }
                    else
                    {
                        var gridObject = new GridObject(_gridSystem, x, y);
                        gridObject.SetPlayable(true);
                        _gridSystem.SetValue(x, y, gridObject);
                    }

                    if (cellData.layer != null)
                    {
                        _pieceSpawner.SpawnLayer(cellData.layer.id, position, transform);
                    }
                }
            }
        }
        private void SpawnBackground(Vector2Int position) => Instantiate(_backgroundTilePrefab, _gridSystem.GetWorldPositionCenter(position.x, position.y), Quaternion.identity, transform);

        private void HandleClick()
        {
            var worldPos = Camera.main.ScreenToWorldPoint(Touchscreen.current.position.ReadValue());
            worldPos.z = 0f;

            Vector2Int gridPos = _gridSystem.GetXY(worldPos);

            if (!IsValidPosition(gridPos))
                return;

            // if (!string.IsNullOrEmpty(_inventoryViewModel.SelectedPowerUpId.Value))
            // {
            //     if (_isBusy) return;
            //     SetBusy(true);
            //     _usePowerUpUseCase.Execute(_inventoryViewModel.SelectedPowerUpId.Value, gridPos);
            //     _inventoryController.Deselect();
            //     _inventoryPresenter.RefreshInventory();
            //         
            //     EndTurnSequence().Forget();
            //     return;
            // }

            _selectedCell = _gridSystem.GetValue(gridPos.x, gridPos.y);
            _selectedCellDebug.Set(_selectedCell);
        }
        

        private void OnSwipeItem(Vector2 worldPos, Vector2Int direction)
        {
            var a = _gridSystem.GetXY(worldPos);
            var b = a + direction;

            if (!IsValidPosition(a) || !IsValidPosition(b))
                return;

            if (IsEmptyPosition(a) || IsEmptyPosition(b))
                return;

            TryHandleMove(a, b).Forget();
        }
        private async UniTask TryHandleMove(Vector2Int a, Vector2Int b)
        {
            if (_isBusy || _gameEnded)
                return;

            SetBusy(true);

            var validMove = await _turnFlowController.ExecuteMove(a, b);

            if (validMove)
            {
                _remainingMoves--;
                EventQueue.Instance.EnqueueEvent(new MovesChangedEventData(EventIds.MovesChanged, _remainingMoves, 1));
            }

            await _turnFlowController.Resolve();

            await EvaluateGameState();

            SetBusy(false);
        }
        
        private async UniTask EvaluateGameState()
        {
            bool isWin = _questSystem.ActiveQuests.All(q => q.IsReached);

            if (isWin)
            {
                await HandleLevelWin();
                return;
            }

            if (_remainingMoves <= 0)
            {
                HandleGameOver();
                return;
            }

            if (!_moveValidator.HasAnyValidMove())
            {
                Debug.Log("⚠ No hay movimientos, reshuffle necesario");
                _shuffleBoardSystem.Shuffle();
            }
        }

        private void HandleGameOver()
        {
            Debug.Log("[MatchManager] Game Over! Moves finished without completing objectives.");
            _gameEnded = true;
            EventQueue.Instance.EnqueueEvent(new EventData(EventIds.ShowLose));
        }

        private async UniTask HandleLevelWin()
        {
            _gameEnded = true;
            _starSystem.ForceMinimumStar();

            _progressRepository.UpdateLevelProgress(
                _levelDataSO.id,
                _starSystem.CurrentStars,
                true);

            if (_remainingMoves > 0)
            {
                await _endGameController.ExecutePostGame(
                    _remainingMoves,
                    moves =>
                    {
                        _remainingMoves = moves;
                        EventQueue.Instance.EnqueueEvent(new MovesChangedEventData(EventIds.MovesChanged, _remainingMoves, 1));
                    });
            }

            var showWinEventData = new ShowWinEventData(
                EventIds.ShowWin,
                _scoreSystem.CurrentScore,
                _starSystem.CurrentStars);

            EventQueue.Instance.EnqueueEvent(showWinEventData);
        }
        
        private bool IsValidPosition(Vector2Int gridPosition) => gridPosition.x >= 0 && gridPosition.x < _levelDataSO.width && gridPosition.y >= 0 && gridPosition.y < _levelDataSO.height;
        private bool IsEmptyPosition(Vector2Int gridPosition) =>
            _gridSystem.GetValue(gridPosition.x, gridPosition.y)?.GetPiece() == null;

        

        
    }
}

[Serializable]
public class GridObjectDebugView
{
    public int x;
    public int y;
    public bool isEmpty;

    public string tileName;
    public string tileType;
    //public PowerUpType powerUp;

    public void Set(GridObject cell)
    {
        if (cell == null)
            return;

        x = cell.X;
        y = cell.Y;
        isEmpty = cell.IsEmpty();

        var tile = cell.GetPiece();
        if (tile != null)
        {
            tileName = tile.name;
            tileType = tile.GetTile().name;
          //  powerUp = tile.HasPowerUp ? tile.PowerUp : PowerUpType.None;
        }
        else
        {
            tileName = "None";
            tileType = "None";
         //   powerUp = PowerUpType.None;
        }
    }

}
