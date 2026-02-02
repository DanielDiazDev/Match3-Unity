using Match3.Scripts.Data.Score;
using Match3.Scripts.Match.Systems.Shapes;

namespace Match3.Scripts.Score
{
    public interface IScoreStrategy
    {
        int CalculateScore(ClassifiedMatch match, ScoreConfigurationSO config, float multiplier);
    }
}
