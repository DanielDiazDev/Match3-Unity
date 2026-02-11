using Match3.Scripts.Data.Item;
using PrimeTween;
using UnityEngine;
using UnityEngine.Pool;

namespace Match3.Scripts.Pieces
{
    public class Layer : MonoBehaviour
    {
        public LayerSO LayerData { get; private set; }
        private SpriteRenderer _spriteRenderer;
        private IObjectPool<Layer> _pool;

        public void SetPool(IObjectPool<Layer> pool)
        {
            _pool = pool;
        }
        public void Init(LayerSO data)
        {
            LayerData = data;
            _spriteRenderer = GetComponent<SpriteRenderer>();
            if (_spriteRenderer != null && data.sprite != null)
            {
                _spriteRenderer.sprite = data.sprite;
                _spriteRenderer.sortingOrder = -1; // Ensure it's behind pieces
            }
        }

        public void DestroyLayer()
        {
            Tween.Scale(transform, Vector3.zero, 0.2f, Ease.InBack)
                .OnComplete(() => _pool.Release(this));
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
