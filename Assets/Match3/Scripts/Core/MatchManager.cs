using Assets.Match3.Scripts.Grid;
using Cysharp.Threading.Tasks;
using Match3.Scripts.Data;
using Match3.Scripts.Data.Item;
using Match3.Scripts.Grid;
using Match3.Scripts.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using Match3.Scripts.Data.Score;
using Match3.Scripts.General;
using Match3.Scripts.Match.Systems;
using Match3.Scripts.Match.Systems.Find;
using Match3.Scripts.Match.Systems.Shapes;
using Match3.Scripts.Match.Systems.Spawner;
using Match3.Scripts.PowerUps;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Match3.Scripts.Core
{
    public class MatchManager : MonoBehaviour
    {
        [SerializeField] private float _cellSize = 1f;
        [SerializeField] private Vector3 _origin =  Vector3.zero;
        [SerializeField] private bool _debug = true;
        [SerializeField] private SwipeDetector  _swipeDetector;
        [SerializeField] private LevelDataSO _levelDataSO;
        private GridSystem<GridObject>  _gridSystem;
        [SerializeField] private PowerUpSystem  _powerUpSystem;
        [SerializeField] private PiecesConfigurationSO  _piecesConfigurationSO; 
        [SerializeField] private ScoreConfigurationSO _scoreConfigurationSO;
        private Match3.Scripts.Score.ScoreSystem _scoreSystem;
        private Match3.Scripts.Quests.QuestSystem _questSystem;
        private bool _isBusy;
        [Header("DEBUG")]
        [SerializeField] private GridObjectDebugView _selectedCellDebug;
        private GridObject _selectedCell;
        [SerializeField] private List<TileSO> _availableTiles;
        private PieceSwapper _pieceSwapper;
        private RefillSystem _refillSystem;
        private PieceSpawner _pieceSpawner;
        private MatchFinder _matchFinder;
        private MatchProcessor _matchProcessor;
        private GravitySystem _gravitySystem;
        private MoveValidator _moveValidator;
        private ShuffleBoardSystem _shuffleBoardSystem;
        private void OnEnable()
        {
            _swipeDetector.OnSwipeDetected += OnSwipeItem;
        }
        private void OnDisable()
        {
            _swipeDetector.OnSwipeDetected -= OnSwipeItem;
        }
        private void Start()
        {
            InitializeSystems();
            InitializeGrid();
        }
        private void Update()
        {
            if (!_debug) return;

            if (Touchscreen.current != null && Touchscreen.current.press.wasPressedThisFrame)
            {
                HandleClick();
            }
        }
        private void InitializeSystems()
        {
            _gridSystem = GridSystem<GridObject>.VerticalGrid(_levelDataSO.width, _levelDataSO.height, _cellSize, _origin, _debug);
            _pieceSpawner = new PieceSpawner(_piecesConfigurationSO, _gridSystem);
            _pieceSwapper = new PieceSwapper(_gridSystem);
            _matchFinder = new MatchFinder(_gridSystem, _levelDataSO.width, _levelDataSO.height);
            _scoreSystem = new Match3.Scripts.Score.ScoreSystem(new Match3.Scripts.Score.Match3ScoreStrategy(), _scoreConfigurationSO);
            _questSystem = new Match3.Scripts.Quests.QuestSystem();
            _questSystem.Initialize(_levelDataSO.quests);
            _scoreSystem.OnScoreChanged += (score) => _questSystem.NotifyScoreUpdated(score);
            
            _powerUpSystem.Init(_gridSystem, _scoreSystem, _questSystem);
            _matchProcessor = new MatchProcessor(_gridSystem, _powerUpSystem, _scoreSystem);
            _gravitySystem = new GravitySystem(_gridSystem, _levelDataSO.width, _levelDataSO.height);
            _refillSystem = new RefillSystem(_levelDataSO, _gridSystem, _pieceSpawner, transform);
            _moveValidator = new MoveValidator(_gridSystem, _matchFinder);
            _shuffleBoardSystem = new ShuffleBoardSystem(_gridSystem, _matchFinder, _moveValidator);
        }
        private void InitializeGrid()
        {
            for (var x = 0; x < _levelDataSO.width; x++) 
            {
                for (var y = 0; y < _levelDataSO.height; y++)
                {
                    var tileSo = _levelDataSO.initialPieces[new Vector2Int(x, y)];
                    var piece = _pieceSpawner.Spawn(tileSo.id, new Vector2Int(x, y), transform);
                    piece.PlaySpawnAnimation();
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

            _isBusy = true;
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
                _isBusy = false;
                return;
            }

            //  Loop de cascadas
            do
            {
                if (matches.Any())
                {
                    await _matchProcessor.ProcessMatches(matches, new List<Vector2Int> { a, b });
                    await CommandQueue.Instance.WaitForAllCommands();
                }

                bool moved;
                do
                {
                    moved = await _gravitySystem.ApplyGravity();
                }
                while (moved);

                await _refillSystem.ApplyRefill();
                ValidateBoard();
                matches = _matchFinder.FindMatches(_gridSystem.GetAllPositions());
                
                if (matches.Any())
                {
                    _scoreSystem.IncrementMultiplier();
                }
            }
            while (matches.Any());

            if (!_moveValidator.HasAnyValidMove())
            {
                Debug.Log("⚠ No hay movimientos, reshuffle necesario");
                _shuffleBoardSystem.Shuffle();
            }

            _isBusy = false;
        }

        public void ValidateBoard()
        {
            foreach (var pos in _gridSystem.GetAllPositions())
            {
                var cell = _gridSystem.GetValue(pos.x, pos.y);
                if (cell.IsEmpty()) continue;

                var expected = _gridSystem.GetWorldPositionCenter(pos.x, pos.y);
                var actual = cell.GetPiece().transform.position;

                if (Vector3.Distance(expected, actual) > 0.01f)
                {
                    Debug.LogError($"Desync en {pos} | Esperado {expected} | Actual {actual}");
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
