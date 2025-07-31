using UnityEngine;

namespace Scriptable_Objects
{
    [CreateAssetMenu(fileName = "Factory Element", menuName = "Factory/Element", order = 0)]
    public class FactoryElement : ScriptableObject
    {
        [SerializeField] private float cost;
        [SerializeField] private GameObject prefab;

        public float Cost => cost;
        public GameObject Prefab => prefab;
    }
}