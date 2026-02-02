using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using NUnit.Framework;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Match3.Scripts.Vfx
{
    public class ParticleManager : MonoBehaviour
    {
        public static ParticleManager Instance;
        [Serializable]
        public class ParticleEntry
        {
            public string id;
            public ParticleSystem prefab;
            public int initialSize = 10;
        }
        [SerializeField] private List<ParticleEntry> _particles;
        private Dictionary<string, ParticlePool> pools = new Dictionary<string, ParticlePool>();

        private void Awake()
        {
            if (Instance != null)
            {
                Destroy(gameObject);
                return;
            }

            Instance = this;
            InitializePools();
        }

        private void InitializePools()
        {
            foreach (var entry in _particles)
            {
                var pool = new ParticlePool(entry.prefab, entry.initialSize, transform);
                pools.Add(entry.id, pool);
            }
        }

        public void Play(string id, Vector3 position, Quaternion rotation = default)
        {
            if (!pools.TryGetValue(id, out var pool))
            {
                Debug.LogWarning($"Particle ID {id} not found.");
                return;
            }

            pool.Play(position, rotation == default ? Quaternion.identity : rotation);
        }
    }

    public class ParticlePool
    {
        private ParticleSystem _prefab;
        private Transform _parent;
        private Queue<ParticleSystem> _pool;

        public ParticlePool(ParticleSystem prefab, int initialSize, Transform parent)
        {
            _prefab = prefab;
            _parent = parent;
            _pool = new Queue<ParticleSystem>();
            for (int i = 0; i < initialSize; i++)
            {
                CreateInstance();
            }
        }

        private ParticleSystem CreateInstance()
        {
            var instance = Object.Instantiate(_prefab, _parent);
            instance.gameObject.SetActive(false);
            _pool.Enqueue(instance);
            return instance;
        }

        public void Play(Vector3 position, Quaternion rotation)
        {
            var ps = _pool.Count > 0 ? _pool.Dequeue() : CreateInstance();
            ps.transform.position = position;
            ps.transform.rotation = rotation;

            ps.gameObject.SetActive(true);
            ps.Play();
            ReturnToPool(ps).Forget();
        }

        private async UniTask ReturnToPool(ParticleSystem ps)
        {
            await UniTask.WaitUntil(() => !ps.IsAlive(true));
            ps.gameObject.SetActive(false);
            _pool.Enqueue(ps);
        }
    }
}