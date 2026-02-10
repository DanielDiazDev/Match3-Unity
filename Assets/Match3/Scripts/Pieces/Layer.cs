using Match3.Scripts.Data.Item;
using PrimeTween;
using UnityEngine;

namespace Match3.Scripts.Pieces
{
    public class Layer : MonoBehaviour
    {
        public LayerSO LayerData { get; private set; }
        private SpriteRenderer _spriteRenderer;

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
                .OnComplete(() => Destroy(gameObject));
        }
    }
}
