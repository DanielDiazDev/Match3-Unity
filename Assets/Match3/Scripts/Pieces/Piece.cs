using Match3.Scripts.Core;
using Match3.Scripts.Data.Item;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Match3.Scripts.Pieces
{
    public class Piece : MonoBehaviour
    {
        public PieceSO pieceSo { get; private set; }

        private SpriteRenderer _spriteRenderer;

        public void Init(PieceSO pieceSo)
        {
            this.pieceSo = pieceSo;
            _spriteRenderer = GetComponent<SpriteRenderer>();
            _spriteRenderer.sprite = pieceSo.sprite;
        }
       
        public PieceSO GetTile() => pieceSo;
        public void OnDestroyed()
        {
            Destroy(gameObject);
        }
    }
}