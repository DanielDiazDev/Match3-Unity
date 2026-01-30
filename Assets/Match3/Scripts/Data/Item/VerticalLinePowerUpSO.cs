using Assets.Match3.Scripts.Grid;
using Match3.Scripts.Grid;
using Match3.Scripts.Pieces;
using Match3.Scripts.PowerUps;
using UnityEngine;

namespace Match3.Scripts.Data.Item
{
    [CreateAssetMenu(fileName = "VerticalLinePowerUpSO", menuName = "Piece/PowerUp/VerticalLinePowerUpSO")]
    public class VerticalLinePowerUpSO : PowerUpSO
    {
        public override void Activate(Piece piece, GridSystem<GridObject> gridSystem, PowerUpSystem powerUpSystem)
        {
            var gridObject = gridSystem.GetValue(piece.transform.position);
            int x = gridObject.X;

            for (int y = 0; y < gridSystem.Height; y++)
            {
                if (y == gridObject.Y) continue;
                powerUpSystem.ClearCell(x, y);
            }
        }
    }
}