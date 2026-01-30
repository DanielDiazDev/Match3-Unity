using Assets.Match3.Scripts.Grid;
using Cysharp.Threading.Tasks;
using Match3.Scripts.Data.Item;
using Match3.Scripts.General;
using Match3.Scripts.Grid;
using Match3.Scripts.Pieces;

namespace Match3.Scripts.PowerUps
{
    public class PowerUpCommand : ICommand
    {
        private PowerUpSO _powerUpSo;
        private GridSystem<GridObject>  _gridSystem;
        private Piece _piece;

        public PowerUpCommand(PowerUpSO powerUpSo, Piece piece, GridSystem<GridObject> gridSystem)
        {
            _powerUpSo = powerUpSo;
            _piece = piece;
            _gridSystem = gridSystem;
        }

        public async UniTask Execute()
        {
            _powerUpSo.Activate(_piece,  _gridSystem);
        }
    }
}