using UnityEngine;

namespace Scriptable_Objects
{
    [CreateAssetMenu(fileName = "New Item", menuName = "Resources/Item")]
    public class ItemType : ResourceType
    {
        [SerializeField] private MarketBehaviour marketBehaviour;
        [SerializeField] private Sprite inWorldSprite;

        public Sprite InWorldSprite => inWorldSprite;

        public MarketBehaviour MarketBehaviour => marketBehaviour;
    }
}