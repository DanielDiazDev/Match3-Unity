using UniRx;

namespace Match3.Scripts.Player
{
    public class PlayerViewModel
    {
        public ReactiveProperty<int> Coins;
        public ReactiveProperty<int> Health;

        public PlayerViewModel()
        {
            Coins = new ReactiveProperty<int>();
            Health = new ReactiveProperty<int>();
        }
    }
}