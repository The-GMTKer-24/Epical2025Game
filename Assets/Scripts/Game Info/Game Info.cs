using System;
using System.Collections.Generic;
using System.Linq;
using Factory_Elements;
using Scriptable_Objects;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Serialization;
using Random = Unity.Mathematics.Random;

namespace Game_Info
{
    public class GameInfo : MonoBehaviour
    {
        public static GameInfo Instance { get; private set; }
        [SerializeField] int initialMoney;
        [SerializeField] QuestSet initialQuests;
        [SerializeField] QuestSet questList;
        [SerializeField] FactoryElementSet startingUnlockedFactoryElements;
        [SerializeField] FactoryElementSet startingFactoryElements;
        private List<Quest> activeQuests;
        private List<FactoryElementType> unlockedFactoryElements;
        private int money;
        public void Awake()
        {
            Instance = this;
            activeQuests = initialQuests.Quests.ToList();
            
            unlockedFactoryElements = startingUnlockedFactoryElements.Elements.ToList();
            money = initialMoney;
            
        }

        public void Start()
        {
            foreach (var startingFactoryElement in startingFactoryElements.Elements)
            {
                bool placed = false;
                int count = 0;
                foreach (var position in Factory.Instance.Bounds.allPositionsWithin)
                {
                    GameObject newObj = Factory.Instance.TryPlace(startingFactoryElement, new int2(position.x, position.y), out placed);
                    if (placed)
                    {
                        newObj.transform.position = (Vector2)position;
                        break;
                    }
                }
            }
        }

        public List<Quest> ActiveQuests => activeQuests;

        public List<FactoryElementType> UnlockedFactoryElements => unlockedFactoryElements;

        public int Money => money;
    }
}