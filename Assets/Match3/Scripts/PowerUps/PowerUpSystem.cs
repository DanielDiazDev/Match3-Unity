using System.Linq;
using System.Threading.Tasks;
using Assets.Match3.Scripts.Grid;
using Match3.Scripts.Data.Item;
using Match3.Scripts.General;
using Match3.Scripts.Grid;
using UnityEngine;

namespace Match3.Scripts.PowerUps
{
    public class PowerUpSystem : MonoBehaviour
    {
        private GridSystem<GridObject> _gridSystem;
        [SerializeField] private PowerUpSODatabase _powerUpSoDatabase;

        public void Init(GridSystem<GridObject> gridSystem)
        {
            _gridSystem = gridSystem;
        }

        public bool HavePowerUpSwap(Vector2Int a,  Vector2Int b)
        {
            return _gridSystem.GetValue(a.x, a.y).HasPowerUp() ||  _gridSystem.GetValue(b.x, b.y).HasPowerUp();
        }

        public void ChangeToPowerUp(Vector2Int position)
        {
          //  var piece = _gridSystem.GetValue(position.x, position.y).GetPiece().ChangePieceSO();
        }

        public void AddPowerUpToCommand(string powerUpId, Vector2Int position)
        {
            var powerUp = _powerUpSoDatabase.powerUpSOs.FirstOrDefault(p => p.id == powerUpId);
            var piece = _gridSystem.GetValue(position.x, position.y).GetPiece();
            CommandQueue.Instance.AddCommand(new PowerUpCommand(powerUp, piece, _gridSystem));
        }
    }
}