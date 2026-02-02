using System.Collections.Generic;
using AYellowpaper.SerializedCollections;
using Match3.Scripts.Match.Systems.Shapes;
using UnityEngine;

namespace Match3.Scripts.Data.Score
{
    [CreateAssetMenu(fileName = "ScoreConfiguration", menuName = "Score/Configuration")]
    public class ScoreConfigurationSO : ScriptableObject
    {
        [SerializeField]
        [SerializedDictionary("Shape", "Points")]
        private SerializedDictionary<MatchShape, int> _shapePoints = new();

        [SerializeField] private int _basePieceValue = 10;
        [SerializeField] private int _powerUpActivationValue = 50;
        [SerializeField] private float _cascadeMultiplierIncrement = 0.5f;

        public int GetPointsForShape(MatchShape shape)
        {
            return _shapePoints.GetValueOrDefault(shape, 0);
        }

        public int BasePieceValue => _basePieceValue;
        public int PowerUpActivationValue => _powerUpActivationValue;
        public float CascadeMultiplierIncrement => _cascadeMultiplierIncrement;
    }
}
