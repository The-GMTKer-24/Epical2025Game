using UnityEngine;

namespace Scriptable_Objects
{
    [CreateAssetMenu(fileName = "New Item", menuName = "Resources/Item")]
    public class ItemType : ResourceType
    {
        [SerializeField] private int cost;
        [SerializeField] private MarketBehaviour marketBehaviour;
        [SerializeField] private Sprite inWorldSprite;


        public override void Reset()
        {
            base.Reset();
            if (missingPlaceholder != null)
                inWorldSprite = missingPlaceholder;
        }

        public override void OnEnable()
        {
            base.OnEnable();
            if (inWorldSprite == null)
                inWorldSprite = missingPlaceholder;
        }
        public Sprite InWorldSprite => inWorldSprite;

        public MarketBehaviour MarketBehaviour => marketBehaviour;
        
        public int Cost => cost;
    }
}