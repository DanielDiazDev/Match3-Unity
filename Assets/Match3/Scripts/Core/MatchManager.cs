using Assets.Match3.Scripts.Grid;
using Cysharp.Threading.Tasks;
using Match3.Scripts.Data.Level;
using Match3.Scripts.Event;
using Match3.Scripts.Event.Data;
using Match3.Scripts.Grid;
using Match3.Scripts.Match.Systems;
using Match3.Scripts.Match.Systems.Find;
using Match3.Scripts.Match.Systems.Spawner;
using Match3.Scripts.Persistence;
using Match3.Scripts.PowerUps;
using Match3.Scripts.Quests;
using Match3.Scripts.Score;
using Match3.Scripts.Sound;
using Match3.Scripts.UI.Game;
using Reflex.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using Match3.Scripts.Core.States;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Match3.Scripts.Core
{
    public class MatchManager : MonoBehaviour, IEventObserver
    {
        [SerializeField] private float _cellSize = 1f;
        [SerializeField] private bool _debug = true;
        [SerializeField] private LevelDataSO _levelDataSoTest;
        [SerializeField] private PowerUpSystem  _powerUpSystem;
        [SerializeField] private PiecesConfigurationSO  _piecesConfigurationSO;
        [SerializeField] private GridObjectDebugView _selectedCellDebug;
        [SerializeField] private GameObject _backgroundTilePrefab;
        [SerializeField] private InventoryUI _inventoryUI;
        [SerializeField] private GameObject _gameEnd;

        private MatchStateContext _ctx;
        private MatchStateMachine _stateMachine;

        private IScoreSystem _scoreSystem;
        private IQuestSystem _questSystem;
        private StarSystem _starSystem;
        private IProgressRepository _progressRepository;

        private GridSystem<GridObject> _gridSystem;
        private PieceSpawner _pieceSpawner;
        private PieceSwapper _pieceSwapper;
        private MatchFinder _matchFinder;
        private MatchProcessor _matchProcessor;
        private GravitySystem _gravitySystem;
        private RefillSystem _refillSystem;
        private MoveValidator _moveValidator;
        private ShuffleBoardSystem _shuffleBoardSystem;
        private BoardResolver _boardResolver;
        private TurnFlowController _turnFlowController;
        private EndGameSequenceController _endGameController;
        private LevelDataSO _levelDataSO;
        public bool IsBusy => _ctx != null && _ctx.IsBusy;
        private bool _gameEnded;
        private GridObject _selectedCell;
        private string _selectedPowerUpId;
        private Vector3 _origin;

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
            if (eventData.EventId != EventIds.SwipeDetected) return;
            var swipe = (SwipeDetectedEventData)eventData;
            OnSwipeItem(swipe.worldStart, swipe.direction);
        }

        private void Start()
        {
            ServiceLocator.ServiceLocator.Global.Get<SoundManager>().PlayMusic(SoundId.GameMusic);
            EventQueue.Instance.Subscribe(EventIds.SwipeDetected, this);
            InitializeSystems();
            InitializeStateMachine();
            _inventoryUI.OnPowerUpSelected += OnPowerUpSelected;
        }

        private void OnPowerUpSelected(string id)
        {
            _selectedPowerUpId = id;
        }

        private void Update()
        {
            if (_ctx == null || _ctx.GameEnded) return;

            if (_stateMachine.CurrentState != MatchState.Idle) return;

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
                _progressRepository = new ProgressRepository();
            }
            else
            {
                _progressRepository = ServiceLocator.ServiceLocator.Global.Get<IProgressRepository>();
                _levelDataSO = _progressRepository.CurrentLevel;
            }

            _origin = new Vector3(-_levelDataSO.width * _cellSize * 0.5f,
                                  -_levelDataSO.height * _cellSize * 0.5f, 0f);
            _gridSystem = GridSystem<GridObject>.VerticalGrid(_levelDataSO.width, _levelDataSO.height,
                                                              _cellSize, _origin, _debug);

            _pieceSpawner   = new PieceSpawner(_piecesConfigurationSO, _gridSystem);
            _pieceSwapper   = new PieceSwapper(_gridSystem);
            _matchFinder    = new MatchFinder(_gridSystem, _levelDataSO.width, _levelDataSO.height);
            _questSystem    = new QuestSystem(_levelDataSO.quests);
            _starSystem     = new StarSystem(_levelDataSO, _questSystem, _scoreSystem);

            _powerUpSystem.Init(_gridSystem, _scoreSystem, _questSystem);
            _matchProcessor = new MatchProcessor(_gridSystem, _powerUpSystem, _scoreSystem);
            _gravitySystem  = new GravitySystem(_gridSystem, _levelDataSO.width, _levelDataSO.height);
            _refillSystem   = new RefillSystem(_levelDataSO, _gridSystem, _pieceSpawner, transform);
            _moveValidator  = new MoveValidator(_gridSystem, _matchFinder);
            _shuffleBoardSystem = new ShuffleBoardSystem(_gridSystem, _matchFinder, _moveValidator);
            _boardResolver  = new BoardResolver(_matchFinder, _matchProcessor, _gravitySystem,
                                                _refillSystem, _gridSystem, _scoreSystem);
            _turnFlowController = new TurnFlowController(_pieceSwapper, _moveValidator, _boardResolver,
                _powerUpSystem, _scoreSystem, _gridSystem, _matchFinder);
            _endGameController  = new EndGameSequenceController(_gridSystem, _powerUpSystem, _boardResolver);

            _inventoryUI.Init(_progressRepository);
        }

        private async UniTask InitializeStateMachineAsync()
        {
            _ctx = new MatchStateContext
            {
                GridSystem           = _gridSystem,
                PieceSpawner         = _pieceSpawner,
                PieceSwapper         = _pieceSwapper,
                MatchFinder          = _matchFinder,
                MatchProcessor       = _matchProcessor,
                GravitySystem        = _gravitySystem,
                RefillSystem         = _refillSystem,
                MoveValidator        = _moveValidator,
                ShuffleBoardSystem   = _shuffleBoardSystem,
                PowerUpSystem        = _powerUpSystem,
                BoardResolver        = _boardResolver,
                TurnFlowController   = _turnFlowController,
                EndGameController    = _endGameController,

                ScoreSystem          = _scoreSystem,
                QuestSystem          = _questSystem,
                StarSystem           = _starSystem,
                ProgressRepository   = _progressRepository,

                LevelDataSO          = _levelDataSO,
                RemainingMoves       = _levelDataSO.maxMoves,
                GameEnded            = false,
                InventoryUI          = _inventoryUI,
                GameEnd              = _gameEnd,
                Transform            = transform,
            };

            _ctx.OnMovesChanged += moves =>
            {
                EventQueue.Instance.EnqueueEvent(new MovesChangedEventData(EventIds.MovesChanged, moves, 1));
            };
            _ctx.OnBusyChanged += busy => {  };

            var states = new Dictionary<MatchState, IMatchState>
            {
                { MatchState.Idle,             new IdleState(_ctx) },
                { MatchState.GeneratePieces,   new GeneratePiecesState(_ctx, _backgroundTilePrefab) },
                { MatchState.Swap,             new SwapState(_ctx) },
                { MatchState.FindMatch,        new FindMatchState(_ctx) },
                { MatchState.Destroy,          new DestroyState(_ctx) },
                { MatchState.PowerUpGeneration,new PowerUpGenerationState(_ctx) },
                { MatchState.PowerUpActivate,  new PowerUpActivateState(_ctx) },
                { MatchState.Gravity,          new GravityState(_ctx) },
                { MatchState.Refill,           new RefillState(_ctx) },
                { MatchState.Shuffle,          new ShuffleState(_ctx) },
                { MatchState.GameOver,         new GameOverState(_ctx) },
                { MatchState.LevelWin,         new LevelWinState(_ctx) },
            };

            _stateMachine = new MatchStateMachine(states);

            var next = await _stateMachine.Initialize(MatchState.GeneratePieces);

            if (next != MatchState.None && next != _stateMachine.CurrentState)
            {
                next = await _stateMachine.TransitionTo(next);
            }

            while (next != MatchState.Idle && next != MatchState.None)
            {
                next = await _stateMachine.TransitionTo(next);
            }

            if (next == MatchState.Idle && _stateMachine.CurrentState != MatchState.Idle)
            {
                await _stateMachine.TransitionTo(MatchState.Idle);
            }
        }

        private void InitializeStateMachine()
        {
            InitializeStateMachineAsync().Forget();
        }


        private void ClearSelectedPowerUp()
        {
            _selectedPowerUpId = null;
        }

        private void HandleClick()
        {
            if (_ctx == null || _ctx.IsBusy || _ctx.GameEnded) return;
            if (_stateMachine.CurrentState != MatchState.Idle) return;

            var worldPos = Camera.main.ScreenToWorldPoint(Touchscreen.current.position.ReadValue());
            worldPos.z = 0f;

            Vector2Int gridPos = _gridSystem.GetXY(worldPos);

            if (!IsValidPosition(gridPos))
                return;

            if (!string.IsNullOrEmpty(_selectedPowerUpId))
            {
                _ctx.SelectedPowerUpId = _selectedPowerUpId;
                _ctx.PendingPowerUpPos = gridPos;
                UsePowerUpAt(gridPos).Forget();
                return;
            }

            _selectedCell = _gridSystem.GetValue(gridPos.x, gridPos.y);
            _selectedCellDebug.Set(_selectedCell);
        }

        private async UniTask UsePowerUpAt(Vector2Int gridPos)
        {
            if (_ctx == null || _ctx.IsBusy || _ctx.GameEnded) return;

            var gridObject = _gridSystem.GetValue(gridPos.x, gridPos.y);
            if (gridObject == null || gridObject.IsEmpty() || !gridObject.IsPlayable())
                return;

            _ctx.SelectedPowerUpId = _selectedPowerUpId;
            _ctx.PendingPowerUpPos = gridPos;
            await RunTurnFrom(MatchState.PowerUpActivate);

            ClearSelectedPowerUp();
        }

        private async UniTask RunTurnFrom(MatchState startState)
        {
            if (_ctx == null || _ctx.IsBusy || _ctx.GameEnded) return;

            var next = await _stateMachine.TransitionTo(startState);

            while (next != MatchState.Idle && next != MatchState.None)
            {
                next = await _stateMachine.TransitionTo(next);
            }

          
            if (next == MatchState.Idle && _stateMachine.CurrentState != MatchState.Idle)
            {
                await _stateMachine.TransitionTo(MatchState.Idle);
            }

            await EvaluateGameStateAfterTurn();
        }

        private void OnSwipeItem(Vector2 worldPos, Vector2Int direction)
        {
            if (_ctx == null || _ctx.IsBusy || _ctx.GameEnded) return;
            if (_stateMachine.CurrentState != MatchState.Idle) return;

            var a = _gridSystem.GetXY(worldPos);
            var b = a + direction;

            if (!IsValidPosition(a) || !IsValidPosition(b))
                return;

            if (IsEmptyPosition(a) || IsEmptyPosition(b))
                return;

            _ctx.PendingSwap = (a, b);
            RunTurnFrom(MatchState.Swap).Forget();
        }

        private async UniTask EvaluateGameStateAfterTurn()
        {
            if (_ctx == null || _ctx.GameEnded) return;

            bool isWin = _questSystem.ActiveQuests.All(q => q.IsReached);

            if (isWin)
            {
                await _stateMachine.TransitionTo(MatchState.LevelWin);
                return;
            }

            if (_ctx.RemainingMoves <= 0)
            {
                await _stateMachine.TransitionTo(MatchState.GameOver);
                return;
            }

            if (!_moveValidator.HasAnyValidMove())
            {
                Debug.Log("⚠ No hay movimientos, reshuffle necesario");
                await _stateMachine.TransitionTo(MatchState.Shuffle);
            }
        }
        
        

        private bool IsValidPosition(Vector2Int gridPosition) =>
            gridPosition.x >= 0 && gridPosition.x < _levelDataSO.width &&
            gridPosition.y >= 0 && gridPosition.y < _levelDataSO.height;

        private bool IsEmptyPosition(Vector2Int gridPosition) =>
            _gridSystem.GetValue(gridPosition.x, gridPosition.y)?.GetPiece() == null;
    }

    [Serializable]
    public class GridObjectDebugView
    {
        public int x;
        public int y;
        public bool isEmpty;

        public string tileName;
        public string tileType;

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
            }
            else
            {
                tileName = "None";
                tileType = "None";
            }
        }
    }
}
