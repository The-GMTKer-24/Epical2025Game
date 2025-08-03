using System;
using System.Collections.Generic;
using System.Linq;
using Factory_Elements;
using Factory_Elements.Blocks;
using Game_Info;
using Player;
using Scriptable_Objects;
using UI.Grid;
using UnityEngine;
using UnityEngine.PlayerLoop;
using UnityEngine.Serialization;
using Buffer = Factory_Elements.Blocks.Buffer;
using Object = UnityEngine.Object;

namespace UI.Inventory
{
    public class InventoryUI : MonoBehaviour
    {
        public static InventoryUI Instance { get; private set; }
        [SerializeField]
        private RectTransform inventoryPanel;
        [SerializeField]
        private RectTransform inventoryContent;
        [SerializeField]
        private RectTransform factoryInventoryPanel;
        [SerializeField]
        private RectTransform factoryInventoryContent;
        [SerializeField]
        private RectTransform marketPanel;
        [SerializeField]
        private RectTransform marketContent;
        [SerializeField]
        private InventorySlot inventorySlotPrefab;
        [SerializeField]
        private MarketSlot marketSlotPrefab;
        [SerializeField]
        private QuestElement questElementPrefab;
        [SerializeField] private ResourceSet sellables;
        
        [SerializeField] private GameObject failSoundPrefab;
        [SerializeField] private GameObject chchingSoundPrefab;
        [SerializeField] private QuestSet allQuests;

        private BuildMode previousBuildMode;

        private SelectableInventory? lastSelectedInventory;
        private InventorySlot previousSlot;
        private ResourceType previousSlotType;
        private BufferBlock bufferBlock;
        
        private bool showing;
        public bool ShowingFactory => factoryInventoryPanel.gameObject.activeInHierarchy;
        public bool Showing => showing;

        private bool questMode;

        public void Awake()
        {
            previousBuildMode = BuildMode.None;
            Instance = this;
            questMode = true;
        }

        public void Start()
        {
            Hide();
        }
        
        public void Show()
        {
            previousBuildMode = GridSystem.Instance.buildMode;
            GridSystem.Instance.SetBuildMode(BuildMode.None);
            PlayerCamera.Instance.EnableQuickMove(false);
            if (showing)
                return;
            showing = true;

            List<KeyValuePair<ResourceType, ResourceStack>> playerInv = Player.Player.Instance.Inventory.ToList();
            playerInv.Add(new KeyValuePair<ResourceType, ResourceStack>(null,null));
            
            inventoryPanel.gameObject.SetActive(true);

            foreach (KeyValuePair<ResourceType, ResourceStack> resources in playerInv)
            {
                inventoryInstanceCount++;
                InventorySlot slot = Instantiate(inventorySlotPrefab, inventoryContent);
                if (resources.Key != null)
                {
                    slot.SetAmount(resources.Value.Quantity);
                    slot.SetHoverText(resources.Value.ResourceType.name);
                    slot.SetSprite(resources.Value.ResourceType.Icon);
                }
                else
                {
                    slot.SetAmount("");
                    slot.SetHoverText("");
                    slot.SetSprite(null);
                }

                slot.OnSelect += (iSlot) =>
                {
                    if (lastSelectedInventory != null)
                    {
                        if (previousSlot.ClicksSinceSelected > 1)
                        {
                            return;
                        }

                        if (lastSelectedInventory == SelectableInventory.Player)
                        {
                            // We dont need to do anything for player=>palyer
                        }
                        else if (lastSelectedInventory == SelectableInventory.Factory)
                        {
                            while (bufferBlock.Buffers[previousSlotType].Quantity > 0)
                            {
                                Player.Player.Instance.AddResource(bufferBlock.Buffers[previousSlotType]
                                    .TakeResource());
                            }

                            ResetPanel();
                            Show(bufferBlock);
                            
                        }
                    }

                    lastSelectedInventory = SelectableInventory.Player;
                    previousSlot = iSlot;
                    previousSlotType = resources.Value?.ResourceType;
                };
            }
            
            inventoryContent.GetComponent<RectTransform>().sizeDelta =
                new Vector2(0, 70 * MathF.Ceiling(inventoryInstanceCount/4f) );
        }

        public void Show(BufferBlock bufferBlock)
        {
            if (showing)
                return;
            Show();
            this.bufferBlock = bufferBlock;
            factoryInventoryPanel.gameObject.SetActive(true);
            foreach (KeyValuePair<ResourceType, Buffer> bufferBlockBuffer in bufferBlock.Buffers)
            {
                factoryInventoryInstanceCount++;
                InventorySlot slot = Instantiate(inventorySlotPrefab, factoryInventoryContent);
                slot.SetAmount(bufferBlockBuffer.Value.Quantity);
                slot.SetHoverText(bufferBlockBuffer.Value.ResourceType.name);
                slot.SetSprite(bufferBlockBuffer.Value.ResourceType.Icon);
                slot.OnUpdate += (slot) =>
                {
                    slot.SetAmount(bufferBlockBuffer.Value.Quantity);
                    slot.SetHoverText(bufferBlockBuffer.Value.ResourceType.name);
                    slot.SetSprite(bufferBlockBuffer.Value.ResourceType.Icon);
                };
                slot.OnSelect += (iSlot) =>
                {
                    if (lastSelectedInventory != null)
                    {
                        if (previousSlot.ClicksSinceSelected > 1)
                        {
                            return;
                        }
                        if (lastSelectedInventory == SelectableInventory.Factory)
                        {
                            // Factory => Factory just doesnt make sense
                        }
                        else if (lastSelectedInventory == SelectableInventory.Player && previousSlotType != null)
                        {
                            if (bufferBlock.Buffers.ContainsKey(previousSlotType))
                            {
                                while (bufferBlock.Buffers[previousSlotType].CanAcceptInput && bufferBlock.Buffers[previousSlotType].Quantity < bufferBlock.Buffers[previousSlotType].Capacity)
                                {
                                    bufferBlock.Buffers[previousSlotType].AddResource(Player.Player.Instance.Inventory[previousSlotType].TakeResource());
                                }
                                ResetPanel();
                                Show(bufferBlock);
                                
                            }
                        }
                    }
                    lastSelectedInventory = SelectableInventory.Factory;
                    previousSlot = iSlot;
                    previousSlotType = bufferBlockBuffer.Value.ResourceType;
                };
            }

            factoryInventoryContent.GetComponent<RectTransform>().sizeDelta =
                new Vector2(0, 70 * MathF.Ceiling(factoryInventoryInstanceCount/4f) );
        }

        private int marketInstanceCount;
        private int inventoryInstanceCount;
        private int factoryInventoryInstanceCount;
        
        public void ShowMarket()
        {
            if (showing)
            {
                return;
            }
            ResetPanel();
            Show();

            marketPanel.gameObject.SetActive(true);
            if (!questMode)
            {
                foreach (var sellableResource in sellables.Resources)
                {
                    ItemType sellableItem =  (ItemType)sellableResource;
                    MarketSlot slot = Instantiate(marketSlotPrefab, marketContent);
                    marketInstanceCount++;
                    marketContent.GetComponent<RectTransform>().sizeDelta = new Vector2(0, 70 * marketInstanceCount);
                    slot.SetName(sellableResource.name);
                    slot.SetSprite(sellableResource.Icon);
                    slot.SetPrice(sellableItem.MarketBehaviour.MaxPrice);

                    if (!Player.Player.Instance.HasItem(sellableResource))
                    {
                        print($"Disabling slot {sellableResource.name}");
                        slot.SetDisabled(true);
                    }

                    slot.OnSelect += (iSlot) =>
                    {
                        if (slot.disabled)
                        {
                            Object failSound= Instantiate(failSoundPrefab);
                            Destroy(failSound, 0.5f);
                            return;
                        }
                    
                        print(sellableItem.MarketBehaviour.MaxPrice);
                        if (Player.Player.Instance.GetResourceAmount(sellableResource) > 0)
                        {
                            ResourceStack stack = Player.Player.Instance.RemoveStack(sellableResource);
                            print(stack.Quantity);

                            GameInfo.Instance.GainMoney((int)(stack.Quantity * sellableItem.MarketBehaviour.MaxPrice));
                        
                            GameObject rejectionSound = Instantiate(failSoundPrefab);
                            Destroy(rejectionSound, 0.5f);
                        }
                    };
                }
            }
            else
            {
                int instanceCount = 0;
                foreach (var quest in allQuests.Quests)
                {
                    instanceCount++;
                    QuestElement questElement = Instantiate(questElementPrefab, marketContent);
                    questElement.SetRewardText(quest.Unlocks,quest.Rewards, quest.MoneyReward);
                    questElement.SpawnItemTrackers(quest.Requirements);
                }
                marketContent.GetComponent<RectTransform>().sizeDelta = new Vector2(0, 160 * instanceCount);
            }
        }

        public void Hide()
        {
            if (inventoryPanel.gameObject.activeInHierarchy)
            {
                GridSystem.Instance.SetBuildMode(previousBuildMode);
            }
            bufferBlock = null;
            
            ResetPanel();
        }

        private void ResetPanel()
        {
            marketInstanceCount = 0;
            inventoryInstanceCount = 0;
            factoryInventoryInstanceCount = 0;
            lastSelectedInventory = null;
            previousSlot = null;
            previousSlotType = null;
            inventoryPanel.gameObject.SetActive(false);
            factoryInventoryPanel.gameObject.SetActive(false);
            marketPanel.gameObject.SetActive(false);
            foreach (Transform child in inventoryContent.transform)
            {
                Destroy(child.gameObject);
            }
            foreach (Transform child in factoryInventoryContent.transform)
            {
                Destroy(child.gameObject);
            }
            foreach (Transform child in marketContent.transform)
            {
                Destroy(child.gameObject);
            }
            showing = false;
        }

        public void Refresh()
        {
            if (ShowingFactory)
            {
                var temp = bufferBlock;
                Hide();
                Show(temp);
            }
            else
            {
                Hide();
                Show();
            }
        }
    }
}