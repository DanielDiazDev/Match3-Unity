using UnityEngine;

namespace Match3.Scripts.General
{
    public class FitSpriteToCamera : MonoBehaviour
    {
        private void Start()
        {
            var cam = Camera.main;

            var camHeight = 2f * cam.orthographicSize;
            var camWidth = camHeight * cam.aspect;

            var sr = GetComponent<SpriteRenderer>();
            var spriteWidth = sr.sprite.bounds.size.x;
            var spriteHeight = sr.sprite.bounds.size.y;

            var scale = transform.localScale;
            scale.x = camWidth / spriteWidth;
            scale.y = camHeight / spriteHeight;
            transform.localScale = scale;
        }
    }
}