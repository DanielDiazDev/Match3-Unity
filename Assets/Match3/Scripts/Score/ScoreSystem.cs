using System;
using Match3.Scripts.Data.Score;
using Match3.Scripts.Event;
using Match3.Scripts.Event.Data;
using Match3.Scripts.Match.Systems.Shapes;
using UnityEngine;

namespace Match3.Scripts.Score
{
    public interface IScoreSystem
    {
        void AddMatchScore(ClassifiedMatch match);
        void AddPieceClearScore();
        void AddPowerUpActivationScore();
        void IncrementMultiplier();
        void ResetMultiplier();
        int CurrentScore { get; }
    }
    public class ScoreSystem : IScoreSystem
    {
        private int _currentScore;
        private float _currentMultiplier = 1f;
        private IScoreStrategy _strategy;
        private ScoreConfigurationSO _config;


        public int CurrentScore => _currentScore;
        public float CurrentMultiplier => _currentMultiplier;

        public ScoreSystem(IScoreStrategy strategy, ScoreConfigurationSO config)
        {
            _strategy = strategy;
            _config = config;
        }

       

        public void AddMatchScore(ClassifiedMatch match)
        {
            int points = _strategy.CalculateScore(match, _config, _currentMultiplier);
            _currentScore += points;
            
            var scoreChangedEventData = new ScoreChangedEventData(EventIds.ScoreChanged, _currentScore);
            EventQueue.Instance.EnqueueEvent(scoreChangedEventData);
            Debug.Log($"[ScoreSystem] Match: {match.shape} ({match.positions.Count} pieces) x{_currentMultiplier} = {points} pts. Total: {_currentScore}");
        }

        public void AddPieceClearScore()
        {
            int points = Mathf.RoundToInt(_config.BasePieceValue * _currentMultiplier);
            _currentScore += points;

            Debug.Log($"[ScoreSystem] PieceClear x{_currentMultiplier} = {points} pts. Total: {_currentScore}");

            var scoreChangedEventData = new ScoreChangedEventData(EventIds.ScoreChanged, _currentScore);
            EventQueue.Instance.EnqueueEvent(scoreChangedEventData);
            Debug.Log($"[ScoreSystem] OnScoreChanged invoked: {_currentScore}");

        }


        public void AddPowerUpActivationScore()
        {
            var points = Mathf.RoundToInt(_config.PowerUpActivationValue * _currentMultiplier);
            _currentScore += points;
            var scoreChangedEventData = new ScoreChangedEventData(EventIds.ScoreChanged, _currentScore);
            EventQueue.Instance.EnqueueEvent(scoreChangedEventData);
            Debug.Log($"[ScoreSystem] PowerUp Activated! x{_currentMultiplier} = {points} pts. Total: {_currentScore}");
        }

        public void IncrementMultiplier()
        {
            _currentMultiplier += _config.CascadeMultiplierIncrement;
            var multiplierScoreChangedEventData = new MultiplierScoreChangedEventData(EventIds.MultiplierChanged, _currentMultiplier);
            EventQueue.Instance.EnqueueEvent(multiplierScoreChangedEventData);
        }

        public void ResetMultiplier()
        {
            _currentMultiplier = 1f;
            var multiplierScoreChangedEventData = new MultiplierScoreChangedEventData(EventIds.MultiplierChanged, _currentMultiplier);
            EventQueue.Instance.EnqueueEvent(multiplierScoreChangedEventData);
        }

        public void ResetScore()
        {
            _currentScore = 0;
            _currentMultiplier = 1f;
            var scoreChangedEventData = new ScoreChangedEventData(EventIds.ScoreChanged, _currentScore);
            EventQueue.Instance.EnqueueEvent(scoreChangedEventData);
        }
    }
}
