using Match3.Scripts.Data.Item;
using PrimeTween;
using UnityEngine;

namespace Match3.Scripts.Pieces
{
    public class Block : MonoBehaviour
    {
        public BlockSO BlockData { get; private set; }
        public int CurrentHealth { get; private set; }

        private SpriteRenderer _spriteRenderer;

        public void Init(BlockSO data)
        {
            BlockData = data;
            CurrentHealth = data.initialHealth;
            _spriteRenderer = GetComponent<SpriteRenderer>();
            if (_spriteRenderer != null && data.sprite != null)
            {
                _spriteRenderer.sprite = data.sprite;
            }
        }

        public void TakeDamage(int amount = 1)
        {
            if (!BlockData.isDestructible) return;

            CurrentHealth -= amount;
            if (CurrentHealth <= 0)
            {
                DestroyBlock();
            }
            else
            {
                PlayHitAnimation();
            }
        }

        private void PlayHitAnimation()
        {
            Tween.Scale(transform, 0.9f, 0.1f, Ease.OutQuad)
                .OnComplete(() => Tween.Scale(transform, 1f, 0.1f, Ease.OutQuad));
        }

        public void DestroyBlock()
        {
             Tween.Scale(transform, Vector3.zero, 0.2f, Ease.InBack)
                .OnComplete(() => Destroy(gameObject));
        }
    }
}
