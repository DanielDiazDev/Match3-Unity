using Assets.Match3.Scripts.Grid;
using Cysharp.Threading.Tasks;
using Match3.Scripts.Data.Item;
using Match3.Scripts.Grid;
using System;
using System.Collections.Generic;
using System.Linq;
using Match3.Scripts.Data.Level;
using Match3.Scripts.Data.Score;
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
using UnityEngine;
using UnityEngine.InputSystem;

namespace Match3.Scripts.Core
{
    public class MatchManager : MonoBehaviour, IEventObserver
    {
        public event Action<int> OnMovesChanged; //No va
        [SerializeField] private float _cellSize = 1f;
        [SerializeField] private bool _debug = true;
        [SerializeField] private LevelDataSO _levelDataSoTest;
        [SerializeField] private PowerUpSystem  _powerUpSystem; //No va
        [SerializeField] private PiecesConfigurationSO  _piecesConfigurationSO; //No va
        [SerializeField] private ScoreConfigurationSO _scoreConfigurationSO;//No va
        private IScoreSystem _scoreSystem;//No va
        private IQuestSystem _questSystem;//No va
        [SerializeField] private InventoryView _inventoryView;//No va
        [SerializeField] private PowerUpSODatabase _powerUpDatabase;//No va
        [SerializeField] private List<TileSO> _availableTiles;//No va
        [SerializeField] private GridObjectDebugView _selectedCellDebug;
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
        private void OnEnable()
        {
            EventQueue.Instance.Subscribe(EventIds.SwipeDetected, this);
        }
        private void OnDestroy()
        {
            EventQueue.Instance.Unsubscribe(EventIds.SwipeDetected, this);
        }
        public void Process(EventData eventData)
        {
            if(eventData.EventId != EventIds.SwipeDetected) return;
            var swipeDetectedEventData = (SwipeDetectedEventData)eventData;
            OnSwipeItem(swipeDetectedEventData.worldStart, swipeDetectedEventData.direction);
        }

        private void Start()
        {
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
            _scoreSystem = new ScoreSystem(new Match3ScoreStrategy(), _scoreConfigurationSO);
            _questSystem = new QuestSystem(_levelDataSO.quests);
            _starSystem = new StarSystem(_levelDataSO, _questSystem, _scoreSystem);

            _remainingMoves = _levelDataSO.maxMoves;
            OnMovesChanged?.Invoke(_remainingMoves);

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
            //InitializeInventory();
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
                    GridObject gridObject = null;
                    
                    // 1. Check for Block
                    if (_levelDataSO.initialBlocks.TryGetValue(position, out var blockSO))
                    {
                        // Spawn Block
                        var blockGo = Instantiate(blockSO.prefab, transform);
                        blockGo.transform.position = _gridSystem.GetWorldPositionCenter(x, y);
                        var block = blockGo.GetComponent<Match3.Scripts.Pieces.Block>();
                        block.Init(blockSO);

                        // Create GridObject with Block
                        gridObject = new GridObject(_gridSystem, x, y);
                        gridObject.SetBlock(block);
                        _gridSystem.SetValue(x, y, gridObject);
                    }
                    else
                    {
                        // 2. Spawn Piece if no Block
                        // Note: If there's no initial piece defined but also no block, we might want to spawn a random one or leave empty/spawn from available.
                        // Existing logic assumed initialPieces has everything. We'll stick to that but fall back or handle missing if needed.
                        // Actually, standard match-3: if it's a valid cell, it usually has a piece unless it's a "hole" (no grid object) or has a block.
                        // For now we assume if not Block, we try to load Piece.
                        
                        if (_levelDataSO.initialPieces.TryGetValue(position, out var tileSo))
                        {
                            var piece = _pieceSpawner.Spawn(tileSo.id, position, transform);
                            piece.PlaySpawnAnimation();
                            // Spawn creates and sets gridObject
                            gridObject = _gridSystem.GetValue(x, y);
                        }
                        else
                        {
                             // Fallback: Create empty grid object if nothing defined (e.g. empty tile)
                             gridObject = new GridObject(_gridSystem, x, y);
                             _gridSystem.SetValue(x, y, gridObject);
                        }
                    }

                    // 3. Check for Layer (can coexist with Piece or Block, though usually behind Piece)
                    if (_levelDataSO.initialLayers.TryGetValue(position, out var layerSO))
                    {
                        var layerGo = Instantiate(layerSO.prefab, transform);
                        layerGo.transform.position = _gridSystem.GetWorldPositionCenter(x, y);
                        var layer = layerGo.GetComponent<Match3.Scripts.Pieces.Layer>();
                        layer.Init(layerSO);

                        gridObject.SetLayer(layer);
                    }
                }
            }
        }
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

            bool validMove = await _turnFlowController.ExecuteMove(a, b);

            if (validMove)
            {
                _remainingMoves--;
                OnMovesChanged?.Invoke(_remainingMoves);
            }

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
                        OnMovesChanged?.Invoke(_remainingMoves);
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
