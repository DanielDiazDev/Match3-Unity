using UniRx;

namespace Match3.Scripts.Level
{
    public class LevelDetailViewModel
    {
        public readonly ReactiveProperty<string> LevelId = new ReactiveProperty<string>();
        public readonly ReactiveProperty<int> Stars = new ReactiveProperty<int>();
        public readonly ReactiveCommand StartLevel = new ReactiveCommand();
    }
}