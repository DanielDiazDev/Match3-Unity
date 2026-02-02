using Match3.Scripts.Data.Item;
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

        public PieceDestructionProgress(PieceDestructionObjectiveSO objective) : base(objective)
        {
            _so = objective;
        }

        public override bool IsReached => _currentAmount >= _so.amountRequired;

        public override string GetProgressFormatted() => $"{_currentAmount}/{_so.amountRequired}";

        public override void Initialize(Match3.Scripts.Quests.QuestSystem questSystem)
        {
            questSystem.OnPieceDestroyed += HandlePieceDestroyed;
        }

        private void HandlePieceDestroyed(PieceSO piece)
        {
            if (piece.id == _so.targetPiece.id)
            {
                _currentAmount++;
                OnProgressChanged?.Invoke();
            }
        }
    }
}
