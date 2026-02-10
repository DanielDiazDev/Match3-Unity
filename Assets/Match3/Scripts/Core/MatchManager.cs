using Assets.Match3.Scripts.Grid;
using Cysharp.Threading.Tasks;
using Match3.Scripts.Data;
using Match3.Scripts.Data.Item;
using Match3.Scripts.Grid;
using Match3.Scripts.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using Match3.Scripts.Data.Level;
using Match3.Scripts.Data.Score;
using Match3.Scripts.Event;
using Match3.Scripts.Event.Data;
using Match3.Scripts.General;
using Match3.Scripts.Match.Systems;
using Match3.Scripts.Match.Systems.Find;
using Match3.Scripts.Match.Systems.Shapes;
using Match3.Scripts.Match.Systems.Spawner;
using Match3.Scripts.Persistence;
using Match3.Scripts.PowerUps;
using Match3.Scripts.Quests;
using Match3.Scripts.Score;
using Match3.Scripts.UI.EndGame;
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
        [SerializeField] private ScoreSystem _scoreSystem;//No va
        [SerializeField] private QuestSystem _questSystem;//No va
        [SerializeField] private InventoryView _inventoryView;//No va
        [SerializeField] private EndGameUI _endGameUI;//No va
        [SerializeField] private PowerUpSODatabase _powerUpDatabase;//No va
        [SerializeField] private List<TileSO> _availableTiles;//No va
        [SerializeField] private GridObjectDebugView _selectedCellDebug;
        private StarSystem _starSystem;
        private InventoryViewModel _inventoryViewModel;//No va
        private InventoryController _inventoryController;//No va
        private InventoryPresenter _inventoryPresenter;//No va
        private UsePowerUpUseCase _usePowerUpUseCase;//No va
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
        private void OnEnable()
        {
            EventQueue.Instance.Subscribe(EventIds.SwipeDetected, this);
        }
        private void OnDestroy()
        {
            _inventoryController?.Dispose();
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

            _origin = new Vector3(-_levelDataSO.width * _cellSize * 0.5f, -_levelDataSO.height * _cellSize * 0.5f, 0f);
            _gridSystem = GridSystem<GridObject>.VerticalGrid(_levelDataSO.width, _levelDataSO.height, _cellSize, _origin, _debug);
            _pieceSpawner = new PieceSpawner(_piecesConfigurationSO, _gridSystem);
            _pieceSwapper = new PieceSwapper(_gridSystem);
            _matchFinder = new MatchFinder(_gridSystem, _levelDataSO.width, _levelDataSO.height);
            _scoreSystem.Init(new Match3ScoreStrategy(), _scoreConfigurationSO);
            _questSystem.Initialize(_levelDataSO.quests);
            _scoreSystem.OnScoreChanged += (score) => _questSystem.NotifyScoreUpdated(score);
            
            _starSystem = new StarSystem(_levelDataSO, _scoreSystem, _questSystem);

            _remainingMoves = _levelDataSO.maxMoves;
            OnMovesChanged?.Invoke(_remainingMoves);

            _powerUpSystem.Init(_gridSystem, _scoreSystem, _questSystem);
            _matchProcessor = new MatchProcessor(_gridSystem, _powerUpSystem, _scoreSystem);
            _gravitySystem = new GravitySystem(_gridSystem, _levelDataSO.width, _levelDataSO.height);
            _refillSystem = new RefillSystem(_levelDataSO, _gridSystem, _pieceSpawner, transform);
            _moveValidator = new MoveValidator(_gridSystem, _matchFinder);
            _shuffleBoardSystem = new ShuffleBoardSystem(_gridSystem, _matchFinder, _moveValidator);

            InitializeInventory();
        }
        private void SetBusy(bool busy)
        {
            _isBusy = busy;
            if (_inventoryViewModel != null)
            {
                _inventoryViewModel.IsBusy.Value = busy;
            }
        }
        //No va aca, installer o manager
        private void InitializeInventory()
        {
           // var progressRepository = ServiceLocator.ServiceLocator.Global.Get<IProgressRepository>();
           if (!ServiceLocator.ServiceLocator.Global.TryGet<IProgressRepository>(out var progressRepository) && _debug)
           {
               _progressRepository = progressRepository = new ProgressRepository();
           }
           _inventoryViewModel = new InventoryViewModel();
           _inventoryView.Configure(_inventoryViewModel);
            
           _inventoryPresenter = new InventoryPresenter(_inventoryViewModel, progressRepository, _powerUpDatabase);
           _inventoryController = new InventoryController(_inventoryViewModel);
           _usePowerUpUseCase = new UsePowerUpUseCase(progressRepository, _powerUpSystem);

           _inventoryPresenter.RefreshInventory();
        }

        private void HandleGameOver()
        {
            Debug.Log("[MatchManager] Game Over! Moves finished without completing objectives.");
            _endGameUI.ShowLose();
        }

        private async UniTask HandleLevelWin()
        {
            _starSystem.ForceMinimumStar();
            Debug.Log($"[MatchManager] Level Win! Stars: {_starSystem.CurrentStars} | Final Star Power: {_starSystem.CurrentStarPower}");

           // var repository = ServiceLocator.ServiceLocator.Global.Get<IProgressRepository>();
            _progressRepository.UpdateLevelProgress(_levelDataSO.id, _starSystem.CurrentStars, true);
            
            if (_remainingMoves > 0)
            {
                await ExecutePostGameSequence();
            }
            
            _endGameUI.ShowWin(_scoreSystem.CurrentScore, _starSystem.CurrentStars);
        }

        //cUANDO cumplimos los objetivos y tenemos movimientos se auto juega
        private async UniTask ExecutePostGameSequence()
        {
            _isBusy = true;
            Debug.Log($"[MatchManager] Executing Post-Game Sequence for {_remainingMoves} remaining moves.");

            while (_remainingMoves > 0)
            {
                _remainingMoves--;
                OnMovesChanged?.Invoke(_remainingMoves);

                // Pick a random piece type available in the grid
                var allPieces = _gridSystem.GetAllPositions()
                    .Select(pos => _gridSystem.GetValue(pos.x, pos.y).GetPiece())
                    .Where(p => p != null)
                    .ToList();

                if (allPieces.Count == 0) break;

                var randomPiece = allPieces[UnityEngine.Random.Range(0, allPieces.Count)];
                var targetId = randomPiece.GetTile().id;

                var positionsToDestroy = _gridSystem.GetAllPositions()
                    .Where(pos => 
                    {
                        var p = _gridSystem.GetValue(pos.x, pos.y).GetPiece();
                        return p != null && p.GetTile().id == targetId;
                    })
                    .ToList();

                Debug.Log($"[MatchManager] Post-Game: Destroying {positionsToDestroy.Count} pieces of type {targetId}");

                foreach (var pos in positionsToDestroy)
                {
                    _powerUpSystem.ClearCell(pos.x, pos.y);
                }

                await CommandQueue.Instance.WaitForAllCommands();
                await UniTask.Delay(1000); // Small pause for visuals

                // Wait for cascades and refills after each "move bonus"
                bool matchesFound;
                do
                {
                    bool moved;
                    do
                    {
                        moved = await _gravitySystem.ApplyGravity();
                    }
                    while (moved);

                    await _refillSystem.ApplyRefill();
                    await UniTask.Yield();
                    
                    var matches = _matchFinder.FindMatches(_gridSystem.GetAllPositions());
                    matchesFound = matches.Any();
                    if (matchesFound)
                    {
                        await _matchProcessor.ProcessMatches(matches);
                        await CommandQueue.Instance.WaitForAllCommands();
                    }
                } while (matchesFound);

                await UniTask.Delay(300);
            }

            SetBusy(false);
        }
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

            if (!string.IsNullOrEmpty(_inventoryViewModel.SelectedPowerUpId.Value))
            {
                if (_isBusy) return;
                SetBusy(true);
                _usePowerUpUseCase.Execute(_inventoryViewModel.SelectedPowerUpId.Value, gridPos);
                _inventoryController.Deselect();
                _inventoryPresenter.RefreshInventory();
                    
                EndTurnSequence().Forget();
                return;
            }

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

            RunGameLoop(a, b).Forget();
        }

        private async UniTaskVoid RunGameLoop(Vector2Int a, Vector2Int b)
        {
            if (_isBusy)
                return;

            SetBusy(true);
            _scoreSystem.ResetMultiplier();
            await _pieceSwapper.SwapTile(a, b);

            bool powerUpActivated = false;
            var cellA = _gridSystem.GetValue(a.x, a.y);
            var cellB = _gridSystem.GetValue(b.x, b.y);

            if (cellA.HasPowerUp())
            {
                _powerUpSystem.TriggerPowerUp(a);
                powerUpActivated = true;
            }
            if (cellB.HasPowerUp())
            {
                _powerUpSystem.TriggerPowerUp(b);
                powerUpActivated = true;
            }

            if (powerUpActivated)
            {
                await CommandQueue.Instance.WaitForAllCommands();
            }

            var matches = _matchFinder.FindMatches(new List<Vector2Int> { a, b });

            if (!matches.Any() && !powerUpActivated)
            {
                await _pieceSwapper.SwapTile(b, a);
                SetBusy(false);
                return;
            }

            if (matches.Any() || powerUpActivated)
            {
                _remainingMoves--;
                OnMovesChanged?.Invoke(_remainingMoves);
            }

            EndTurnSequence(new List<Vector2Int> { a, b }).Forget();
        }

        private async UniTaskVoid EndTurnSequence(List<Vector2Int> initialMatches = null)
        {
            await ProcessCascades(initialMatches);

            bool isWin = _questSystem.ActiveQuests.All(q => q.IsReached);

            if (isWin)
            {
                await HandleLevelWin();
            }
            else if (_remainingMoves <= 0)
            {
                HandleGameOver();
            }
            else if (!_moveValidator.HasAnyValidMove())
            {
                Debug.Log("⚠ No hay movimientos, reshuffle necesario");
                _shuffleBoardSystem.Shuffle();
            }

            SetBusy(false);
        }

        private async UniTask ProcessCascades(List<Vector2Int> initialMatchesPositions = null)
        {
            var matches = initialMatchesPositions != null 
                ? _matchFinder.FindMatches(initialMatchesPositions) 
                : _matchFinder.FindMatches(_gridSystem.GetAllPositions());

            do
            {
                if (matches.Any())
                {
                    await _matchProcessor.ProcessMatches(matches, initialMatchesPositions);
                    await CommandQueue.Instance.WaitForAllCommands();
                }

                bool moved;
                do
                {
                    moved = await _gravitySystem.ApplyGravity();
                }
                while (moved);

                await _refillSystem.ApplyRefill();
                await UniTask.Yield(); // Wait for transforms to update properly
                ValidateBoard();
                
                matches = _matchFinder.FindMatches(_gridSystem.GetAllPositions());

                if (matches.Any())
                {
                    _scoreSystem.IncrementMultiplier();
                }

                initialMatchesPositions = null; // Clear initial positions for subsequent matches in cascade
            }
            while (matches.Any());
        }

        public void ValidateBoard()
        {
            foreach (var pos in _gridSystem.GetAllPositions())
            {
                var cell = _gridSystem.GetValue(pos.x, pos.y);
                if (cell.IsEmpty()) continue;

                var expected = _gridSystem.GetWorldPositionCenter(pos.x, pos.y);
                var actualPiece = cell.GetPiece();
                if (actualPiece == null)
                {
                    continue;
                }

                var actual = actualPiece.transform.position;

                if (Vector3.Distance(expected, actual) > 0.05f) // Increased tolerance slightly
                {
                    var piece = cell.GetPiece();
                    Debug.LogError($"[MatchManager] Desync at {pos} | Piece: {piece.name} ({piece.pieceSo.id}) | Expected: {expected} | Actual: {actual} | Dist: {Vector3.Distance(expected, actual)}");
                }
            }
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
