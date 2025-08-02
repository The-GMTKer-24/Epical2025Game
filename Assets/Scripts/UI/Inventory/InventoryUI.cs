using System;
using System.Collections.Generic;
using Factory_Elements;
using Factory_Elements.Blocks;
using Scriptable_Objects;
using UnityEngine;
using Buffer = Factory_Elements.Blocks.Buffer;

namespace UI.Inventory
{
    public class InventoryUI : MonoBehaviour
    {
        public static InventoryUI Instance { get; private set; }
        [SerializeField]
        private RectTransform inventoryPanel;
        [SerializeField]
        private RectTransform factoryElementPanel;
        [SerializeField]
        private InventorySlot inventorySlotPrefab;
        
        public void Awake()
        {
            Instance = this;
        }

        public void Show()
        {
            inventoryPanel.gameObject.SetActive(true);
            foreach (Transform child in inventoryPanel.transform)
            {
                Destroy(child.gameObject);
            }

            for (int i = 0; i < Player.Player.Instance.MaxInventorySize; i++)
            {
                InventorySlot slot = Instantiate(inventorySlotPrefab, inventoryPanel);
                if (i < Player.Player.Instance.Inventory.Count)
                {
                    ResourceStack inventoryStack = Player.Player.Instance.Inventory[i];
                    slot.SetAmount(inventoryStack.Quantity);
                    slot.SetHoverText(inventoryStack.ResourceType.name);
                    slot.SetSprite(inventoryStack.ResourceType.Icon);
                }
            }
        }

        public void Show(BufferBlock bufferBlock)
        {
            Show();
            factoryElementPanel.gameObject.SetActive(true);
            foreach (Transform child in factoryElementPanel.transform)
            {
                Destroy(child.gameObject);
            }

            foreach (KeyValuePair<ResourceType, Buffer> bufferBlockBuffer in bufferBlock.Buffers)
            {
                InventorySlot slot = Instantiate(inventorySlotPrefab, factoryElementPanel);
                slot.SetAmount(bufferBlockBuffer.Value.Quantity);
                slot.SetHoverText(bufferBlockBuffer.Value.ResourceType.name);
                slot.SetSprite(bufferBlockBuffer.Value.ResourceType.Icon);
            }
            
        }

        public void Hide()
        {
            inventoryPanel.gameObject.SetActive(false);
            factoryElementPanel.gameObject.SetActive(false);
            
        }
        


    }
}