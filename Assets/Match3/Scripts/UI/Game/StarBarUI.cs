using System;
using Match3.Scripts.Event;
using Match3.Scripts.Event.Data;
using UnityEngine;
using UnityEngine.UI;

namespace Match3.Scripts.UI.Game
{
    public class StarBarUI : MonoBehaviour, IEventObserver
    {
        [Header("References")]
        [SerializeField] private Image fillImage;
        [SerializeField] private RectTransform fillArea; 
        [SerializeField] private Image[] starImages;

        [Header("Sprites")]
        [SerializeField] private Sprite starOffSprite;
        [SerializeField] private Sprite starOnSprite;

        [SerializeField] private float fillSpeed = 5f;

        
        private float _targetFill;
        private float _currentFill;
        private int _maxThreshold;
        private int[] _thresholds;

        private void Start()
        {
            EventQueue.Instance.Subscribe(EventIds.StarsInitialized,  this);
            EventQueue.Instance.Subscribe(EventIds.StarPowerChanged,  this);
            EventQueue.Instance.Subscribe(EventIds.StarReachedChanged,  this);
        }

        private void OnDestroy()
        {
            EventQueue.Instance.Unsubscribe(EventIds.StarsInitialized,  this);
            EventQueue.Instance.Unsubscribe(EventIds.StarPowerChanged,  this);
            EventQueue.Instance.Unsubscribe(EventIds.StarReachedChanged,  this);
        }

        public void Process(EventData eventData)
        {
            if (eventData.EventId == EventIds.StarsInitialized)
            {
                var starPowerInitializedEventData = (StarPowerInitializedEventData)eventData;
                _thresholds = starPowerInitializedEventData.StarThreshold;
                _maxThreshold = _thresholds[^1];
                Canvas.ForceUpdateCanvases();
            }
            if (eventData.EventId == EventIds.StarPowerChanged)
            {
                var starPowerChangedEventData = (StarPowerChangedEventData)eventData;
                HandleStarPowerChanged(starPowerChangedEventData.CurrentStarPower);
            }
            if (eventData.EventId == EventIds.StarReachedChanged)
            {
                var starReachedEventData = (StarReachedEventData)eventData;
                HandleStarReached(starReachedEventData.CurrentStar);
            }
            
        }
      
        private void HandleStarPowerChanged(int starPower)
        {
            _targetFill = Mathf.Clamp01((float)starPower / _maxThreshold);
        }

        private void HandleStarReached(int starCount)
        {
            for (int i = 0; i < starCount; i++)
            {
                starImages[i].sprite = starOnSprite;
            }
        }

        private void Update()
        {
            if (Mathf.Abs(_currentFill - _targetFill) > 0.001f)
            {
                _currentFill = Mathf.Lerp(_currentFill, _targetFill, Time.deltaTime * fillSpeed);
                fillImage.fillAmount = _currentFill;
            }
        }
    }
}