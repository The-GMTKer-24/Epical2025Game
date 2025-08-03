using System;
using System.Collections.Generic;
using System.Linq;
using Factory_Elements;
using Factory_Elements.Blocks;
using Player;
using Scriptable_Objects;
using UnityEngine;
using UnityEngine.PlayerLoop;
using UnityEngine.Serialization;
using Buffer = Factory_Elements.Blocks.Buffer;

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

        private bool previousBuildMode;

        private SelectableInventory? lastSelectedInventory;
        private InventorySlot previousSlot;
        private ResourceType previousSlotType;
        private BufferBlock bufferBlock;
        
        private bool showing;
        public void Awake()
        {
            Instance = this;
        }

        public void Start()
        {
            Hide();
        }
        
        public void Show()
        {
            previousBuildMode = GridSystem.Instance.buildMode;
            GridSystem.Instance.SetBuildMode(false);
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

        private int marketInstanceCount=0;
        private int inventoryInstanceCount=0;
        private int factoryInventoryInstanceCount=0;
        
        public void ShowMarket()
        {
            if (showing)
            {
                return;
            }
            ResetPanel();
            Show();
            MarketSlot slot = Instantiate(marketSlotPrefab, marketContent);
            marketInstanceCount++;
            marketContent.GetComponent<RectTransform>().sizeDelta = new Vector2(0, 70 * marketInstanceCount);
            slot.SetName("Lighty weather cut copper stairs");
            slot.SetPrice(300);
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
            // marketInstanceCount = 0;
            inventoryInstanceCount = 0;
            factoryInventoryInstanceCount = 0;
            lastSelectedInventory = null;
            previousSlot = null;
            previousSlotType = null;
            inventoryPanel.gameObject.SetActive(false);
            factoryInventoryPanel.gameObject.SetActive(false);
            foreach (Transform child in inventoryContent.transform)
            {
                Destroy(child.gameObject);
            }
            foreach (Transform child in factoryInventoryContent.transform)
            {
                Destroy(child.gameObject);
            }
            showing = false;
        }
    }
}