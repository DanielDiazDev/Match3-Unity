using System.Linq;
using System.Threading.Tasks;
using Assets.Match3.Scripts.Grid;
using Match3.Scripts.Data.Item;
using Match3.Scripts.General;
using Match3.Scripts.Grid;
using Match3.Scripts.Quests;
using Match3.Scripts.Score;
using Match3.Scripts.Vfx;
using PrimeTween;
using UnityEngine;

namespace Match3.Scripts.PowerUps
{
    public class PowerUpSystem : MonoBehaviour
    {
        private GridSystem<GridObject> _gridSystem;
        private IScoreSystem _scoreSystem;
        private IQuestSystem _questSystem;
        [SerializeField] private PowerUpSODatabase _powerUpSoDatabase;

        public void Init(GridSystem<GridObject> gridSystem, IScoreSystem scoreSystem, IQuestSystem questSystem)
        {
            _gridSystem = gridSystem;
            _scoreSystem = scoreSystem;
            _questSystem = questSystem;
        }

        public bool HavePowerUpSwap(Vector2Int a,  Vector2Int b)
        {
            return _gridSystem.GetValue(a.x, a.y).HasPowerUp() ||  _gridSystem.GetValue(b.x, b.y).HasPowerUp();
        }

        public void ChangeToPowerUp(string powerUpId, Vector2Int position)
        {
            var powerUpSo = _powerUpSoDatabase.powerUpSOs.FirstOrDefault(p => p.id == powerUpId);
            if (powerUpSo == null) return;

            var gridObject = _gridSystem.GetValue(position.x, position.y);
            var piece = gridObject.GetPiece();
            piece.ChangePieceSO(powerUpSo);

            // Punch scale for juice
            Tween.PunchScale(piece.transform, Vector3.one * 0.5f, 0.5f);
        }

        public void TriggerPowerUp(Vector2Int position)
        {
            var gridObject = _gridSystem.GetValue(position.x, position.y);
            if (gridObject == null || gridObject.IsEmpty() || !gridObject.HasPowerUp()) return;

            var piece = gridObject.GetPiece();
            var powerUp = piece.pieceSo as PowerUpSO;
            
            _scoreSystem.AddPowerUpActivationScore();
            gridObject.Clear();
            piece.OnDestroyed();

            CommandQueue.Instance.AddCommand(new PowerUpCommand(powerUp, piece, _gridSystem, this));
        }

        public void ClearCell(int x, int y)
        {
            var gridObject = _gridSystem.GetValue(x, y);
            if (gridObject == null || gridObject.IsEmpty()) return;

            if (gridObject.HasPowerUp())
            {
                TriggerPowerUp(new Vector2Int(x, y));
            }
            var piece = gridObject.GetPiece();
            if(piece == null) return;
            
            _scoreSystem.AddPieceClearScore();
            _questSystem.NotifyPieceDestroyed(piece.GetTile());
            piece.OnDestroyed();
            gridObject.Clear();
            ParticleManager.Instance.Play("NormalDestroy", piece.transform.position, Quaternion.identity);
        }
    }
}