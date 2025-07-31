using UnityEngine;

namespace Scriptable_Objects
{
    [CreateAssetMenu(fileName = "New Quest", menuName = "Quests/Quest", order = 0)]
    public class Quest : ScriptableObject
    {
        [SerializeField] private Quest[] prerequisites;
        [SerializeField] private ItemQuantity[] requirements;
        [SerializeField] private ItemQuantity[] rewards;
        [SerializeField] private FactoryElementType[] unlocks;

        public Quest[] Prerequisites => prerequisites;

        public ItemQuantity[] Requirements => requirements;

        public ItemQuantity[] Rewards => rewards;

        public FactoryElementType[] Unlocks => unlocks;
    }
}