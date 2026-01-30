using System.Collections.Generic;
using Assets.Match3.Scripts.Grid;
using Match3.Scripts.Grid;
using Match3.Scripts.Pieces;
using UnityEngine;

namespace Match3.Scripts.Data.Item
{
    public abstract class PowerUpSO : PieceSO
    {
        public GameObject vfxPrefab;
        public AudioClip activateSfx;
        
        public abstract void Activate(Piece piece, GridSystem<GridObject> gridSystem);
    }
    
}