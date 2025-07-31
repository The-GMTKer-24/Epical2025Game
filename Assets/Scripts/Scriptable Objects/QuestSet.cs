using UnityEngine;

namespace Scriptable_Objects
{
    [CreateAssetMenu(fileName = "New Quest Set", menuName = "Sets/Quest Set", order = 0)]
    public class QuestSet : ScriptableObject
    {
        [SerializeField] private Quest[] quests;

        public Quest[] Quests => quests;
    }
}