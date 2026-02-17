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
        private int _remaining;
        private QuestSystem _questSystem;

        public int Remaining => _remaining;
        public int Required => _so.amountRequired;

        public override bool IsReached => _remaining <= 0;

        public PieceSO TargetPiece => _so.targetPiece;

        public PieceDestructionProgress(PieceDestructionObjectiveSO objective) : base(objective)
        {
            _so = objective;
            _remaining = _so.amountRequired;
        }

        public override void Initialize(QuestSystem questSystem)
        {
            _questSystem = questSystem;
            _questSystem.OnPieceDestroyed += HandlePieceDestroyed;
        }

        private void HandlePieceDestroyed(PieceSO piece)
        {
            if (IsReached) return;

            if (piece == _so.targetPiece)
            {
                _remaining = Mathf.Max(0, _remaining - 1);
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
