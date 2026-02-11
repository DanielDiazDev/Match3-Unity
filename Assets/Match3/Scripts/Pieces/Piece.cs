using Match3.Scripts.Core;
using Match3.Scripts.Data.Item;
using System;
using System.Collections.Generic;
using Match3.Scripts.General;
using PrimeTween;
using UnityEngine;
using UnityEngine.Pool;

namespace Match3.Scripts.Pieces
{
    public class Piece : MonoBehaviour, IPoolable
    {
        public PieceSO pieceSo { get; private set; }

        private SpriteRenderer _spriteRenderer;
        private IObjectPool<Piece> _pool;

        public void SetPool(IObjectPool<Piece> pool)
        {
            _pool = pool;
        }

        public void Init(PieceSO pieceSo)
        {
            this.pieceSo = pieceSo;
            _spriteRenderer = GetComponent<SpriteRenderer>();
            _spriteRenderer.sprite = pieceSo.sprite;
        }
       
        public PieceSO GetTile() => pieceSo;

        public void ChangePieceSO(PieceSO newPieceSo)
        {
            this.pieceSo = newPieceSo;
            _spriteRenderer.sprite = pieceSo.sprite;
        }

        public Tween PlaySpawnAnimation()
        {
            transform.localScale = Vector3.zero;
            return Tween.Scale(transform, Vector3.one, 0.3f, Ease.OutBack);
        }

        public void Release()
        {
            _pool.Release(this);
        }

        public Tween PlayDestroyAnimation()
        {
            return Tween.Scale(transform, Vector3.zero, 0.2f, Ease.InBack)
                .OnComplete(Release);
        }

        public void OnDestroyed()
        {
            PlayDestroyAnimation();
        }

        public void OnSpawned()
        {
            transform.localScale = Vector3.one;
            gameObject.SetActive(true);
        }

        public void OnDespawned()
        {
            gameObject.SetActive(false);
        }
    }
}