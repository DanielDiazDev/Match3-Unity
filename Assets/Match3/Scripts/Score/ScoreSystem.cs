using System;
using Match3.Scripts.Data.Score;
using Match3.Scripts.Match.Systems.Shapes;
using UnityEngine;

namespace Match3.Scripts.Score
{
    public class ScoreSystem : MonoBehaviour
    {
        private int _currentScore;
        private float _currentMultiplier = 1f;
        private IScoreStrategy _strategy;
        private ScoreConfigurationSO _config;

        public event Action<int> OnScoreChanged;
        public event Action<float> OnMultiplierChanged;

        public int CurrentScore => _currentScore;
        public float CurrentMultiplier => _currentMultiplier;

        public void Init(IScoreStrategy strategy, ScoreConfigurationSO config)
        {
            _strategy = strategy;
            _config = config;
        }

        public void AddMatchScore(ClassifiedMatch match)
        {
            int points = _strategy.CalculateScore(match, _config, _currentMultiplier);
            _currentScore += points;
            
            OnScoreChanged?.Invoke(_currentScore);
            Debug.Log($"[ScoreSystem] Match: {match.shape} ({match.positions.Count} pieces) x{_currentMultiplier} = {points} pts. Total: {_currentScore}");
        }

        public void AddPieceClearScore()
        {
            int points = Mathf.RoundToInt(_config.BasePieceValue * _currentMultiplier);
            _currentScore += points;

            Debug.Log($"[ScoreSystem] PieceClear x{_currentMultiplier} = {points} pts. Total: {_currentScore}");

            OnScoreChanged?.Invoke(_currentScore);
            Debug.Log($"[ScoreSystem] OnScoreChanged invoked: {_currentScore}");

        }


        public void AddPowerUpActivationScore()
        {
            var points = Mathf.RoundToInt(_config.PowerUpActivationValue * _currentMultiplier);
            _currentScore += points;
            OnScoreChanged?.Invoke(_currentScore);
            Debug.Log($"[ScoreSystem] PowerUp Activated! x{_currentMultiplier} = {points} pts. Total: {_currentScore}");
        }

        public void IncrementMultiplier()
        {
            _currentMultiplier += _config.CascadeMultiplierIncrement;
            OnMultiplierChanged?.Invoke(_currentMultiplier);
        }

        public void ResetMultiplier()
        {
            _currentMultiplier = 1f;
            OnMultiplierChanged?.Invoke(_currentMultiplier);
        }

        public void ResetScore()
        {
            _currentScore = 0;
            _currentMultiplier = 1f;
            OnScoreChanged?.Invoke(_currentScore);
        }
    }
}
