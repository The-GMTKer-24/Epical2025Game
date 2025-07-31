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
            activeQuests = initialQuests.Quests.ToList();
            unlockedFactoryElements = startingFactoryElements.Elements.ToList();
            money = initialMoney;
            Instance = this;
            if (Instance != null) throw new Exception("Multiple instances exist, replacing old instance");
        }

        public List<Quest> ActiveQuests => activeQuests;

        public List<FactoryElementType> UnlockedFactoryElements => unlockedFactoryElements;

        public int Money => money;
    }
}