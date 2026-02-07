using Match3.Scripts.Persistence;
using Match3.Scripts.PowerUps;
using UnityEngine;

namespace Match3.Scripts.UI.Inventory
{
    public class UsePowerUpUseCase
    {
        private readonly IProgressRepository _progressRepository;
        private readonly PowerUpSystem _powerUpSystem;

        public UsePowerUpUseCase(IProgressRepository progressRepository, PowerUpSystem powerUpSystem)
        {
            _progressRepository = progressRepository;
            _powerUpSystem = powerUpSystem;
        }

        public void Execute(string powerUpId, Vector2Int position)
        {
            // 1. Cambiar la pieza por el power-up
            _powerUpSystem.ChangeToPowerUp(powerUpId, position);

            // 2. Activar el power-up inmediatamente
            _powerUpSystem.TriggerPowerUp(position);

            // 3. Consumir del inventario
            _progressRepository.ConsumePowerUp(powerUpId);
        }
    }
}
