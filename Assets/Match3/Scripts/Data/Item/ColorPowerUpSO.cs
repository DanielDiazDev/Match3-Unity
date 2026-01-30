using Assets.Match3.Scripts.Grid;
using Match3.Scripts.Grid;
using Match3.Scripts.Pieces;
using UnityEngine;

namespace Match3.Scripts.Data.Item
{
    [CreateAssetMenu(fileName = "ColorPowerUpSO", menuName = "Piece/PowerUp/ColorPowerUpSO")]
    public class ColorPowerUpSO : PowerUpSO
    {
        public override void Activate(Piece piece, GridSystem<GridObject> gridSystem)
        {
            Debug.Log("ColorPowerUpSO Activate");
        }
    }
}