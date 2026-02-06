using UniRx;

namespace Match3.Scripts.UI.Score
{
    public class ScoreViewModel
    {
        public readonly ReactiveProperty<int> Score = new ReactiveProperty<int>(0);
        public readonly ReactiveProperty<float> Multiplier = new ReactiveProperty<float>(1f);
    }
}