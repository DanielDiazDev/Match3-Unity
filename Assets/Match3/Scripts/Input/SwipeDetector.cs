using System;
using Match3.Scripts.Event;
using Match3.Scripts.Event.Data;
using UnityEngine;

namespace Match3.Scripts.Input
{
    public class SwipeDetector : MonoBehaviour
    {
        private Camera _camera;
        private Vector2 _startPosition;
        private bool _swipeConsumed;
        [SerializeField] private InputReader _inputReader;
        public event Action<Vector2, Vector2Int> OnSwipeDetected;

        private float SwipeThreshold =>
            Screen.dpi > 0 ? Screen.dpi * 0.2f : Screen.height * 0.05f;

        private void OnEnable()
        {
            _inputReader.OnTouchStarted += OnTouchStarted;
            _inputReader.OnTouchEnded += OnTouchEnded;
            _inputReader.EnablePlayerAction();
        }

        private void OnDisable()
        {
            _inputReader.OnTouchStarted -= OnTouchStarted;
            _inputReader.OnTouchEnded -= OnTouchEnded;
        }

        private void Start()
        {
            _camera = Camera.main;
        }

        public void OnTouchStarted(Vector2 position)
        {
            _swipeConsumed = false;
            _startPosition = position;
        }

        public void OnTouchEnded(Vector2 position)
        {
            if (_swipeConsumed) return;

            var delta = position - _startPosition;
            if (delta.magnitude < SwipeThreshold) return;

            _swipeConsumed = true;

            Vector2Int direction =
                Mathf.Abs(delta.x) > Mathf.Abs(delta.y)
                    ? (delta.x > 0 ? Vector2Int.right : Vector2Int.left)
                    : (delta.y > 0 ? Vector2Int.up : Vector2Int.down);

            Vector2 worldStart = _camera.ScreenToWorldPoint(_startPosition);
            //OnSwipeDetected?.Invoke(worldStart, direction);
            var swipeDetectedEventData = new SwipeDetectedEventData(EventIds.SwipeDetected, worldStart, direction);
            EventQueue.Instance.EnqueueEvent(swipeDetectedEventData);
            
        }
    }
}