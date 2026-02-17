using System;
using Match3.Scripts.Event;
using Match3.Scripts.Event.Data;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;

namespace Match3.Scripts.UI.Game
{
    public class ScoreUI : MonoBehaviour, IEventObserver
    {
        [SerializeField] private TMP_Text _scoreText;
        [SerializeField] private float _animationSpeed = 10f;

        private int _displayedScore;
        private int _targetScore;


        private void Start()
        {
            _scoreText.text = "0";
            EventQueue.Instance.Subscribe(EventIds.ScoreChanged, this);
        }

        private void OnDisable()
        {
            EventQueue.Instance.Unsubscribe(EventIds.ScoreChanged, this);
        }

        private void Update()
        {
            AnimateScore();
        }
        public void Process(EventData eventData)
        {
            switch (eventData.EventId)
            {
                case EventIds.ScoreChanged:
                    HandleScoreChanged((ScoreChangedEventData)eventData);
                    break;

                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private void HandleScoreChanged(ScoreChangedEventData data)
        {
            _targetScore = data.Score;
        }

   
        private void AnimateScore()
        {
            if (_displayedScore >= _targetScore)
                return;

            int difference = _targetScore - _displayedScore;

            _displayedScore += Mathf.CeilToInt(difference * _animationSpeed * Time.deltaTime);

            if (_displayedScore > _targetScore)
                _displayedScore = _targetScore;

            _scoreText.text = _displayedScore.ToString("N0");
        }

    }
}