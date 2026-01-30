using Assets.Match3.Scripts.Grid;
using Match3.Scripts.Grid;
using Match3.Scripts.Pieces;
using UnityEngine;

namespace Match3.Scripts.Data.Item
{
    [CreateAssetMenu(fileName = "BombPowerUpSO", menuName = "Piece/PowerUp/BombPowerUpSO")]
    public class BombPowerUpSO : PowerUpSO
    {
        [SerializeField] private int radius = 1;

        public override void Activate(Piece piece, GridSystem<GridObject> gridSystem)
        {
            Debug.Log("BombPowerUpSO Activate");
        }
    }
}