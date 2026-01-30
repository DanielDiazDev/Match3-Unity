using System;
using Assets.Match3.Scripts.Grid;
using Match3.Scripts.Grid;
using Match3.Scripts.Pieces;
using Match3.Scripts.PowerUps;
using UnityEngine;

namespace Match3.Scripts.Data.Item
{
    [CreateAssetMenu(fileName = "ColorPowerUpSO", menuName = "Piece/PowerUp/ColorPowerUpSO")]
    public class ColorPowerUpSO : PowerUpSO
    {
        public override void Activate(Piece piece, GridSystem<GridObject> gridSystem, PowerUpSystem powerUpSystem)
        {
            // For now, it will pick a random color from the grid to clear.
            // In a more advanced implementation, this would be the color it was swapped with.
            string targetColorId = null;

            // Find the first non-powerup piece to get a color
            for (int x = 0; x < gridSystem.Width; x++)
            {
                for (int y = 0; y < gridSystem.Height; y++)
                {
                    var obj = gridSystem.GetValue(x, y);
                    if (obj != null && !obj.IsEmpty() && !obj.HasPowerUp())
                    {
                        targetColorId = obj.GetPiece().pieceSo.id;
                        break;
                    }
                }
                if (targetColorId != null) break;
            }

            if (targetColorId == null) return;

            for (int x = 0; x < gridSystem.Width; x++)
            {
                for (int y = 0; y < gridSystem.Height; y++)
                {
                    var obj = gridSystem.GetValue(x, y);
                    if (obj != null && !obj.IsEmpty() && obj.GetPiece().pieceSo.id == targetColorId)
                    {
                        if (x == gridSystem.GetXY(piece.transform.position).x && 
                            y == gridSystem.GetXY(piece.transform.position).y) continue;

                        powerUpSystem.ClearCell(x, y);
                    }
                }
            }
        }
    }
}