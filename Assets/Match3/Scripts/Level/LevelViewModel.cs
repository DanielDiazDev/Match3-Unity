using UniRx;

namespace Match3.Scripts.Level
{
    public class LevelViewModel
    {
        public readonly ReactiveProperty<string> LevelId =  new ReactiveProperty<string>();
        public readonly ReactiveProperty<int> Stars =   new ReactiveProperty<int>();
        public readonly ReactiveProperty<bool> Unlocked  = new ReactiveProperty<bool>();
        public readonly ReactiveCommand<string> LevelButtonPressed = new ReactiveCommand<string>();
    }
}