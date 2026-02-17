using System;
using System.Collections.Generic;
using Match3.Scripts.Data.Item;
using Match3.Scripts.Persistence;
using Match3.Scripts.Player;
using Match3.Scripts.Utils;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Match3.Scripts.UI.Game
{
    public class InventoryUI : MonoBehaviour
    {
        [SerializeField] private PowerUpSODatabase _powerUpDatabase;
        [SerializeField] private PowerUpItemUI _itemPrefab;
        [SerializeField] private Transform _container;
        [SerializeField] private int _maxSlots = 3;
        [SerializeField] private Sprite _emptySlotSprite;
        private IProgressRepository _repository;
        private string _selectedId;
        private List<PowerUpItemUI> _spawnedItems = new();
        public Action<string> OnPowerUpSelected;

        public void Init(IProgressRepository repository)
        {
            _repository = repository;
            EventDispatcherService.Instance.Subscribe<PlayerModel>(OnPlayerUpdated);
            Refresh();
        }

        private void OnPlayerUpdated(PlayerModel model)
        {
            Refresh();
        }

        private void OnDestroy()
        {
            EventDispatcherService.Instance.Unsubscribe<PlayerModel>(OnPlayerUpdated);
        }


        public void Refresh()
        {
            Clear();

            var purchased = _repository.GetPurchasedPowerUps();

            int spawnedCount = 0;

            // Instanciar powerups comprados
            foreach (var so in _powerUpDatabase.powerUpSOs)
            {
                if (spawnedCount >= _maxSlots)
                    break;

                if (purchased != null &&
                    purchased.TryGetValue(so.id, out int amount) &&
                    amount > 0)
                {
                    var item = Instantiate(_itemPrefab, _container);
                    item.Setup(so, amount);
                    item.OnClicked += HandleSelection;
                    _spawnedItems.Add(item);

                    spawnedCount++;
                }
            }

            for (int i = spawnedCount; i < _maxSlots; i++)
            {
                var emptyItem = Instantiate(_itemPrefab, _container);
                emptyItem.SetupEmpty(_emptySlotSprite);
                _spawnedItems.Add(emptyItem);
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