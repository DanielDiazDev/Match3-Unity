using Assets.Match3.Scripts.Grid;
using Match3.Scripts.Grid;
using Match3.Scripts.Pieces;
using UnityEngine;

namespace Match3.Scripts.Data.Item
{
    [CreateAssetMenu(fileName = "VerticalLinePowerUpSO", menuName = "Piece/PowerUp/VerticalLinePowerUpSO")]
    public class VerticalLinePowerUpSO : PowerUpSO
    {
        public override void Activate(Piece piece, GridSystem<GridObject> gridSystem)
        {
            Debug.Log("VerticalLinePowerUpSO Activate");
        }
    }
}