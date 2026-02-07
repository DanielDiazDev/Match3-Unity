namespace Match3.Scripts.Player
{
    public class PlayerModel
    {
        public int Coins { get; private set; }
        public int Health { get; private set; }

        public PlayerModel(int coins, int health)
        {
            Coins = coins;
            Health = health;
        }

        public bool CanAfford(int price) => Coins >= price;

        public void SpendCoins(int amount) => Coins -= amount;
        public void AddCoins(int amount) => Coins += amount;
        public void AddHealth(int amount) => Health += amount;
    }
}