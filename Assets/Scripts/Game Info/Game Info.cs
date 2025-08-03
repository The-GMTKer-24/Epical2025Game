using System;
using System.Collections.Generic;
using System.Linq;
using Factory_Elements;
using JetBrains.Annotations;
using Scriptable_Objects;
using UnityEngine;

namespace Game_Info
{
    public class GameInfo : MonoBehaviour
    {
        [SerializeField] private int initialMoney;
        [SerializeField] private QuestSet initialQuests;
        [SerializeField] private QuestSet questList;
        public HashSet<String> completedQuests;
        [SerializeField] private FactoryElementSet startingUnlockedFactoryElements;
        public static GameInfo Instance { get; private set; }

        public List<Quest> ActiveQuests { get; private set; }

        public Dictionary<ResourceType, int> SubmittedItems;

        public List<FactoryElementType> UnlockedFactoryElements { get; private set; }

        public int Money { get; private set; }

        public void Awake()
        {
            completedQuests = new HashSet<String>();
            Instance = this;
            ActiveQuests = initialQuests.Quests.ToList();
            SubmittedItems = new Dictionary<ResourceType, int>();

            
            UnlockedFactoryElements = startingUnlockedFactoryElements.Elements.ToList();
            Money = initialMoney;
        }



        public void CompleteQuest(Quest questCompleted)
        {
            completedQuests.Add(questCompleted.name);
            foreach (var unlock in questCompleted.Unlocks)
            {
                UnlockedFactoryElements.Add(unlock);
            }

            foreach (var quantity in questCompleted.Rewards)
            {
                // This is dumb. But there isn't really a better way to give the player items
                for (int i = 0; i < quantity.Amount; i++)
                {
                    Player.Player.Instance.AddResource(Resource.fromType( quantity.Type));
                }
            }
            
            GainMoney(questCompleted.MoneyReward);
        }

        public void SpendMoney(int amount)
        {
            Money -= amount;
        }

        public void GainMoney(int amount)
        {
            Money += amount;
        }
    }
}