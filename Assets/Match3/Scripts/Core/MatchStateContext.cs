using System;
using System.Collections.Generic;
using Assets.Match3.Scripts.Grid;
using Match3.Scripts.Data.Level;
using Match3.Scripts.Grid;
using Match3.Scripts.Match.Systems;
using Match3.Scripts.Match.Systems.Find;
using Match3.Scripts.Match.Systems.Spawner;
using Match3.Scripts.Persistence;
using Match3.Scripts.PowerUps;
using Match3.Scripts.Quests;
using Match3.Scripts.Score;
using Match3.Scripts.UI.Game;
using UnityEngine;

namespace Match3.Scripts.Core
{
    public class MatchStateContext
    {
        // Systems
        public GridSystem<GridObject> GridSystem;
        public PieceSpawner PieceSpawner;
        public PieceSwapper PieceSwapper;
        public MatchFinder MatchFinder;
        public MatchProcessor MatchProcessor;
        public GravitySystem GravitySystem;
        public RefillSystem RefillSystem;
        public MoveValidator MoveValidator;
        public ShuffleBoardSystem ShuffleBoardSystem;
        public PowerUpSystem PowerUpSystem;
        public BoardResolver BoardResolver;
        public TurnFlowController TurnFlowController;
        public EndGameSequenceController EndGameController;

        // Services
        public IScoreSystem ScoreSystem;
        public IQuestSystem QuestSystem;
        public StarSystem StarSystem;
        public IProgressRepository ProgressRepository;

        // Data
        public LevelDataSO LevelDataSO;
        public int RemainingMoves;
        public bool GameEnded;

        // Input / Selection
        public GridObject SelectedCell;
        public string SelectedPowerUpId;

        // Pending actions set by input and consumed by states
        public (Vector2Int a, Vector2Int b)? PendingSwap;
        public Vector2Int? PendingPowerUpPos;

        // Resultados compartidos entre estados (ej. matches encontrados en Swap)
        public List<PieceMatch> LastMatches;

        // UI
        public InventoryUI InventoryUI;
        public GameObject GameEnd;
        public Transform Transform;

        // Lifecycle
        public bool IsBusy;

        // Events
        public event Action<int> OnMovesChanged;
        public event Action<bool> OnBusyChanged;

        public void SetBusy(bool busy)
        {
            IsBusy = busy;
            OnBusyChanged?.Invoke(busy);
        }

        public void NotifyMovesChanged(int moves)
        {
            RemainingMoves = moves;
            OnMovesChanged?.Invoke(moves);
        }

        public void ClearPendingSwap() => PendingSwap = null;
        public void ClearPendingPowerUp() => PendingPowerUpPos = null;
        public void ClearLastMatches() => LastMatches = null;
    }
}
