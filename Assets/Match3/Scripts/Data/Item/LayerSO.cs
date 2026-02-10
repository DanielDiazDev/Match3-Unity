using UnityEngine;

namespace Match3.Scripts.Data.Item
{
    [CreateAssetMenu(fileName = "NewLayer", menuName = "Match3/Items/Layer")]
    public class LayerSO : ScriptableObject
    {
        public string id;
        public Sprite sprite;
        public GameObject prefab;
        public AudioClip destroySound;
    }
}
