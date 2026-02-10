using UnityEngine;

namespace Match3.Scripts.Data.Item
{
    [CreateAssetMenu(fileName = "NewBlock", menuName = "Match3/Items/Block")]
    public class BlockSO : ScriptableObject
    {
        public string id;
        public Sprite sprite;
        public GameObject prefab;
        public int initialHealth = 1;
        public bool isDestructible = true;
        public AudioClip hitSound;
        public AudioClip destroySound;
    }
}
