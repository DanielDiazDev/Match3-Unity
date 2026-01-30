using System;
using System.Collections.Generic;
using Match3.Scripts.Data.Item;
using UnityEngine;

namespace Match3.Scripts.Match.Systems.Find
{
    public struct PieceMatch
    {
        public PieceSO pieceSo;
        public List<Vector2Int> positions;

        public PieceMatch(PieceSO pieceSo, List<Vector2Int> positions)
        {
            this.pieceSo = pieceSo;
            this.positions = positions;
        }
    }
}