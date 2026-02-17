using System.Collections.Generic;
using System.Linq;
using Match3.Scripts.Data.Item;
using Match3.Scripts.Persistence;
using UnityEngine;

namespace Match3.Scripts.UI.Game
{
    public class InventoryUI : MonoBehaviour
    {
        [SerializeField] private PowerUpSODatabase _powerUpDatabase;
        [SerializeField] private PowerUpItemUI _itemPrefab;
        [SerializeField] private Transform _container;

        private IProgressRepository _progressRepository;

        private void Awake()
        {
            _progressRepository = new ProgressRepository();
        }

        private void OnEnable()
        {
            Refresh();
        }

        public void Refresh()
        {
            Clear();

            var purchased = _progressRepository.GetPurchasedPowerUps();

            if (purchased == null || purchased.Count == 0)
                return;

            List<(PowerUpSO data, int amount)> ownedPowerUps = new();

            foreach (var powerUpSO in _powerUpDatabase.powerUpSOs)
            {
                if (purchased.TryGetValue(powerUpSO.id, out int amount) && amount > 0)
                {
                    ownedPowerUps.Add((powerUpSO, amount));
                }
            }

            if (ownedPowerUps.Count == 0)
                return;

            ownedPowerUps = ownedPowerUps
                .OrderBy(x => Random.value)
                .ToList();

            int count = Mathf.Min(3, ownedPowerUps.Count);

            for (int i = 0; i < count; i++)
            {
                var item = Instantiate(_itemPrefab, _container);
                item.Setup(ownedPowerUps[i].data, ownedPowerUps[i].amount);
            }
        }

        private void Clear()
        {
            for (int i = _container.childCount - 1; i >= 0; i--)
            {
                Destroy(_container.GetChild(i).gameObject);
            }
        }
    }
}