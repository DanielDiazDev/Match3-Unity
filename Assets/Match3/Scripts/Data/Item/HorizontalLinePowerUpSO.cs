using System;
using Assets.Match3.Scripts.Grid;
using Match3.Scripts.Grid;
using Match3.Scripts.Pieces;
using Match3.Scripts.PowerUps;
using UnityEngine;

namespace Match3.Scripts.Data.Item
{
    [CreateAssetMenu(fileName = "HorizontalLinePowerUpSO", menuName = "Piece/PowerUp/HorizontalLinePowerUpSO")]
    public class HorizontalLinePowerUpSO : PowerUpSO
    {
        public override void Activate(Piece piece, GridSystem<GridObject> gridSystem, PowerUpSystem powerUpSystem)
        {
            var gridObject = gridSystem.GetValue(piece.transform.position);
            int y = gridObject.Y;

            for (int x = 0; x < gridSystem.Width; x++)
            {
                if (x == gridObject.X) continue;
                powerUpSystem.ClearCell(x, y);
            }
        }
    }
}