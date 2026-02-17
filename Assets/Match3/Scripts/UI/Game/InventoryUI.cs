using System;
using System.Collections.Generic;
using System.Linq;
using Match3.Scripts.Data.Item;
using Match3.Scripts.Persistence;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Match3.Scripts.UI.Game
{
    public class InventoryUI : MonoBehaviour
    {
        [SerializeField] private PowerUpSODatabase _powerUpDatabase;
        [SerializeField] private PowerUpItemUI _itemPrefab;
        [SerializeField] private Transform _container;

        private IProgressRepository _repository;
        private string _selectedId;
        private List<PowerUpItemUI> _spawnedItems = new();
        public Action<string> OnPowerUpSelected;

        public void Init(IProgressRepository repository)
        {
            _repository = repository;
            Refresh();
        }


        public void Refresh()
        {
            Clear();

            var purchased = _repository.GetPurchasedPowerUps();
            if (purchased == null || purchased.Count == 0) return;

            var owned = new List<(PowerUpSO data, int amount)>();

            foreach (var so in _powerUpDatabase.powerUpSOs)
            {
                if (purchased.TryGetValue(so.id, out int amount) && amount > 0)
                {
                    owned.Add((so, amount));
                }
            }

            owned = owned.OrderBy(x => Random.value).ToList();
            int count = Mathf.Min(3, owned.Count);

            for (int i = 0; i < count; i++)
            {
                var item = Instantiate(_itemPrefab, _container);
                item.Setup(owned[i].data, owned[i].amount);

                item.OnClicked += HandleSelection;

                _spawnedItems.Add(item);
            }
        }

        private void HandleSelection(string id)
        {
            if (_selectedId == id)
            {
                Deselect();
                return;
            }

            _selectedId = id;

            foreach (var item in _spawnedItems)
            {
                item.SetSelected(item.Id == id);
            }

            OnPowerUpSelected?.Invoke(id);
        }

        public void Deselect()
        {
            _selectedId = null;

            foreach (var item in _spawnedItems)
                item.SetSelected(false);

            OnPowerUpSelected?.Invoke(null);
        }

        private void Clear()
        {
            foreach (var item in _spawnedItems)
                Destroy(item.gameObject);

            _spawnedItems.Clear();
        }
    }
}