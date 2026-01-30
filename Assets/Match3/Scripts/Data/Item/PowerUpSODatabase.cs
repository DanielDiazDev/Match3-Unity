using System.Collections.Generic;
using UnityEngine;

namespace Match3.Scripts.Data.Item
{
    [CreateAssetMenu(fileName = "PowerUpSODatabase", menuName = "Piece/PowerUp/PowerUpSODatabase")]
    public class PowerUpSODatabase : ScriptableObject
    {
        [field: SerializeField] public List<PowerUpSO> powerUpSOs;
    }
}