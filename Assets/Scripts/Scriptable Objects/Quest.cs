using UnityEngine;

namespace Scriptable_Objects
{
    [CreateAssetMenu(fileName = "New Quest", menuName = "Quests/Quest", order = 0)]
    public class Quest : ScriptableObject
    {
        [SerializeField] private Quest[] prerequisites;
        [SerializeField] private ResourceQuantity[] requirements;
        [SerializeField] private ResourceQuantity[] rewards;
        [SerializeField] private FactoryElementType[] unlocks;

        public Quest[] Prerequisites => prerequisites;

        public ResourceQuantity[] Requirements => requirements;

        public ResourceQuantity[] Rewards => rewards;

        public FactoryElementType[] Unlocks => unlocks;
    }
}