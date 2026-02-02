using Match3.Scripts.Data.Score;
using Match3.Scripts.Match.Systems.Shapes;
using UnityEngine;

namespace Match3.Scripts.Score
{
    public class Match3ScoreStrategy : IScoreStrategy
    {
        public int CalculateScore(ClassifiedMatch match, ScoreConfigurationSO config, float multiplier)
        {
            int shapePoints = config.GetPointsForShape(match.shape);
            return Mathf.RoundToInt(shapePoints * multiplier);
        }
    }
}
