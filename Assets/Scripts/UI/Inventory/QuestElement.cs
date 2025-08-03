using System;
using Scriptable_Objects;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using Object = System.Object;

namespace UI.Inventory
{
    public class QuestElement : MonoBehaviour
    {
        [SerializeField] RectTransform requirementContent;
        [SerializeField] private TextMeshProUGUI rewardText;
        [SerializeField] private ItemCountUpToDate counterPrefab;


        private float mouseEnterAt;
        private bool mouseOver;
        private bool selected;
        public bool disabled { get; private set; }

        public int ClicksSinceSelected { get; private set; }

        
        private PlayerControls controls;


        private void Awake()
        {
            selected = false;
            disabled = false;
            controls = new PlayerControls();
        }

        public void OnEnable()
        {
            controls.Enable();
        }
        public void OnDisable()
        {
            controls.Disable();
        }

      
        

        public void SetRewardText(FactoryElementType[] factoryElements, ResourceQuantity[] resources, int moneyReward)
        {
            String rewardTextVal = "Rewards: ";
            rewardTextVal += $"${moneyReward}";
            foreach (var element in factoryElements)
            {
                rewardTextVal += $", {element.name}";
            }
            foreach (var element in resources)
            {
                rewardTextVal += $", {element.Type.name}*{element.Amount}";
            }

            rewardText.text = rewardTextVal;
        }

        public void SpawnItemTrackers(ResourceQuantity[] questRequirements)
        {
            int instanceCount = 0;

            foreach (var resource in questRequirements)
            {
                instanceCount++;
                ItemCountUpToDate questElement = Instantiate(counterPrefab, requirementContent);
                questElement.targetItem = (ItemType)resource.Type;
                questElement.targetCount = resource.Amount;
            }
            
        }
    }
}