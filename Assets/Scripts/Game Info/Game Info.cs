using System;
using System.Collections.Generic;
using System.Linq;
using Scriptable_Objects;
using UnityEngine;
using UnityEngine.Serialization;

namespace Game_Info
{
    public class GameInfo : MonoBehaviour
    {
        public static GameInfo Instance { get; private set; }
        [SerializeField] int initialMoney;
        [SerializeField] QuestSet initialQuests;
        [SerializeField] QuestSet questList;
        [SerializeField] FactoryElementSet startingFactoryElements;

        private List<Quest> activeQuests;
        private List<FactoryElementType> unlockedFactoryElements;
        private int money;
        public void Awake()
        {
            Instance = this;
            
            activeQuests = initialQuests.Quests.ToList();
            unlockedFactoryElements = startingFactoryElements.Elements.ToList();
            money = initialMoney;

        }

        public List<Quest> ActiveQuests => activeQuests;

        public List<FactoryElementType> UnlockedFactoryElements => unlockedFactoryElements;

        public int Money => money;
    }
}