using System;
using UnityEngine;

namespace Scriptable_Objects
{
    [CreateAssetMenu(fileName = "New Quest Set", menuName = "Sets/Quest Set", order = 0)]
    public class QuestSet : ScriptableObject
    {
        [SerializeField] private Quest[] quests = Array.Empty<Quest>();

        public Quest[] Quests => quests;
    }
}