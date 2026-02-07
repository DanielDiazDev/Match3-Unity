using Match3.Scripts.Data.Item;

namespace Match3.Scripts.UI.Inventory
{
    public class InventoryPowerUpModel
    {
        public string Id { get; }
        public int Quantity { get; set; }
        public PowerUpSO PowerUpSO { get; }

        public InventoryPowerUpModel(string id, int quantity, PowerUpSO powerUpSO)
        {
            Id = id;
            Quantity = quantity;
            PowerUpSO = powerUpSO;
        }
    }
}
