using Match3.Scripts.Data.Item;
using Match3.Scripts.Quests;
using UnityEngine;

namespace Match3.Scripts.Data.Quests
{
    [CreateAssetMenu(fileName = "PieceDestructionObjective", menuName = "Quests/Objectives/PieceDestruction")]
    public class PieceDestructionObjectiveSO : QuestObjectiveSO
    {
        public PieceSO targetPiece;
        public int amountRequired;

        public override QuestProgress CreateProgress()
        {
            return new PieceDestructionProgress(this);
        }
    }

    public class PieceDestructionProgress : QuestProgress
    {
        private readonly PieceDestructionObjectiveSO _so;
        private int _currentAmount;
        private QuestSystem _questSystem;

        public PieceDestructionProgress(PieceDestructionObjectiveSO objective) : base(objective)
        {
            _so = objective;
        }

        public override bool IsReached => _currentAmount >= _so.amountRequired;

        public override string GetProgressFormatted() => $"{_currentAmount}/{_so.amountRequired}";

        public override void Initialize(QuestSystem questSystem)
        {
            _questSystem = questSystem;
            _questSystem.OnPieceDestroyed += HandlePieceDestroyed;
        }

        private void HandlePieceDestroyed(PieceSO piece)
        {
            if (piece.id == _so.targetPiece.id)
            {
                _currentAmount++;
                OnProgressChanged?.Invoke();
            }
        }

        public override void Dispose()
        {
            if (_questSystem != null)
                _questSystem.OnPieceDestroyed -= HandlePieceDestroyed;
        }
    }

}
