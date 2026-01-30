using UnityEngine;

namespace Match3.Scripts.Data.Item
{
    public class PieceSO : ScriptableObject
    {
        public string id;
        public Sprite sprite;
        public GameObject prefab;
        public AudioClip audioClip;
    }
    
}
