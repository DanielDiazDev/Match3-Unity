using Assets.Match3.Scripts.Grid;
using Match3.Scripts.Grid;
using Match3.Scripts.Pieces;
using UnityEngine;

namespace Match3.Scripts.Data.Item
{
    [CreateAssetMenu(fileName = "HorizontalLinePowerUpSO", menuName = "Piece/PowerUp/HorizontalLinePowerUpSO")]
    public class HorizontalLinePowerUpSO : PowerUpSO
    {
        public override void Activate(Piece piece, GridSystem<GridObject> gridSystem)
        {
            Debug.Log("HorizontalLinePowerUpSO Activate");
        }
    }
}