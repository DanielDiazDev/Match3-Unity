using System;
using Assets.Match3.Scripts.Grid;
using Match3.Scripts.Grid;
using Match3.Scripts.Pieces;
using Match3.Scripts.PowerUps;
using UnityEngine;

namespace Match3.Scripts.Data.Item
{
    [CreateAssetMenu(fileName = "BombPowerUpSO", menuName = "Piece/PowerUp/BombPowerUpSO")]
    public class BombPowerUpSO : PowerUpSO
    {
        [SerializeField] private int radius = 1;

        public override void Activate(Piece piece, GridSystem<GridObject> gridSystem, PowerUpSystem powerUpSystem)
        {
            var gridObject = gridSystem.GetValue(piece.transform.position);
            int centerX = gridObject.X;
            int centerY = gridObject.Y;

            for (int x = centerX - radius; x <= centerX + radius; x++)
            {
                for (int y = centerY - radius; y <= centerY + radius; y++)
                {
                    if (x < 0 || x >= gridSystem.Width || y < 0 || y >= gridSystem.Height)
                        continue;

                    if (x == centerX && y == centerY) continue;

                    powerUpSystem.ClearCell(x, y);
                }
            }
        }
    }
}