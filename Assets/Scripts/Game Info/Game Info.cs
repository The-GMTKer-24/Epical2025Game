using System.Collections.Generic;
using System.Linq;
using Scriptable_Objects;
using UnityEngine;

namespace Game_Info
{
    public class GameInfo : MonoBehaviour
    {
        [SerializeField] private int initialMoney;
        [SerializeField] private QuestSet initialQuests;
        [SerializeField] private QuestSet questList;
        [SerializeField] private FactoryElementSet startingUnlockedFactoryElements;
        [SerializeField] private FactoryElementSet startingFactoryElements;
        public static GameInfo Instance { get; private set; }

        public List<Quest> ActiveQuests { get; private set; }

        public List<FactoryElementType> UnlockedFactoryElements { get; private set; }

        public int Money { get; private set; }

        public void Awake()
        {
            Instance = this;
            ActiveQuests = initialQuests.Quests.ToList();

            UnlockedFactoryElements = startingUnlockedFactoryElements.Elements.ToList();
            Money = initialMoney;
        }
    }
}