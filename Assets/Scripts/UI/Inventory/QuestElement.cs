using System;
using System.Collections.Generic;
using Game_Info;
using Scriptable_Objects;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using Object = System.Object;

namespace UI.Inventory
{
    public class QuestElement : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
    {
        [SerializeField] RectTransform requirementContent;
        [SerializeField] private TextMeshProUGUI rewardText;
        [SerializeField] private ItemCountUpToDate counterPrefab;
        [SerializeField] private Image backgroundImage;
        [SerializeField] private Color unclaimableColor;
        [SerializeField] private Color claimableColor;
        [SerializeField] private Color claimHoverColor;
        public Quest quest;

        private float mouseEnterAt;
        private bool mouseOver;
        private bool selected;
        private List<ItemCountUpToDate> requirements;
        public bool disabled { get; private set; }

        public int ClicksSinceSelected { get; private set; }

        
        private PlayerControls controls;


        private void Awake()
        {
            selected = false;
            disabled = false;
            controls = new PlayerControls();
            controls.UI.Click.performed += OnPlayerClick;
            requirements = new List<ItemCountUpToDate>();
        }

        public void OnEnable()
        {
            controls.Enable();
        }
        public void OnDisable()
        {
            controls.Disable();
        }

        private bool claimer=false;
        private void Update()
        {
            bool canClaim = true;

            foreach (var counter in requirements)
            {
                if (!counter.HasCompleted())
                {
                    canClaim = false;
                }
            }
            
            // If we get this far mouse is over and claiming is possible
            if (canClaim)
            {
                backgroundImage.color = claimableColor;
                if (mouseOver)
                {
                    backgroundImage.color = claimHoverColor;
                    claimer = true;
                    return;
                } 
            }
            else
            {
                backgroundImage.color = unclaimableColor;
            }

            claimer = false;
        }

        public void OnPlayerClick(InputAction.CallbackContext ctx)
        {
            print("click");
                
            if (claimer)
            {
                GameInfo.Instance.CompleteQuest(quest);
                InventoryUI.Instance.RefreshQuestsPlusMarket();
            }
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
                requirements.Add(questElement);
                questElement.targetItem = (ItemType)resource.Type;
                questElement.targetCount = resource.Amount;
            }
            requirementContent.GetComponent<RectTransform>().sizeDelta = new Vector2(0, 40 * instanceCount);
            
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            mouseOver = true;   
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            mouseOver = false;
        }
    }
}