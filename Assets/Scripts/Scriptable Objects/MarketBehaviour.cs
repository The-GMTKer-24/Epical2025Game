using UnityEngine;

namespace Scriptable_Objects
{
    [CreateAssetMenu(fileName = "New Market Behaviour", menuName = "Resources/Market Behaviour", order = 0)]
    public class MarketBehaviour : ScriptableObject
    {
        [SerializeField] private int standardPrice;
        [SerializeField] private int minPrice;
        [SerializeField] private int maxPrice;

        public int StandardPrice => standardPrice;

        public int MinPrice => minPrice;

        public int MaxPrice => maxPrice;
    }
}