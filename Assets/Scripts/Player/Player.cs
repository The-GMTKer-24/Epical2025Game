using System;
using System.Collections.Generic;
using System.Linq;
using Factory_Elements;
using Factory_Elements.Blocks;
using Scriptable_Objects;
using UI.Grid;
using UI.Inventory;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Player
{
    public class Player : MonoBehaviour
    {
        [SerializeField] private float speed;
        private readonly Dictionary<ResourceType, ResourceStack> inventory = new();
        private PlayerControls playerControls;
        public static Player Instance { get; private set; }
        
        public Dictionary<ResourceType, ResourceStack> Inventory => inventory;
        private Rigidbody2D rigidbody;

        private void Awake()
        {
            Instance = this;
            rigidbody = GetComponent<Rigidbody2D>();
            playerControls = new PlayerControls();
            playerControls.Player.Interact.performed += ctx => OpenInventoryWindow(ctx,false);
            playerControls.Player.Cancel.performed += OnEscapePressed;
            playerControls.Player.DeleteModeToggle.performed += OnEscapePressed;
            playerControls.Player.BuildModeToggle.performed += OnEscapePressed;
            playerControls.Player.PlaceMachine.performed += context =>
            {
                if (GridSystem.Instance.buildMode == BuildMode.None)
                    OpenInventoryWindow(context,true);
                
            };
        }



        // Update is called once per frame
        private void FixedUpdate()
        {
            var scaledInput = playerControls.Player.Move.ReadValue<Vector2>() * speed;
            if (MathF.Abs(scaledInput.x) > 0) transform.localScale = new Vector3(-MathF.Sign(scaledInput.x), 1, 1);
            //transform.transform.position += new Vector3(scaledInput.x, scaledInput.y, 0);
            rigidbody.AddRelativeForce(scaledInput, ForceMode2D.Impulse);
        }

        private void OnEnable()
        {
            playerControls.Enable();
        }

        private void OnDisable()
        {
            playerControls.Disable();
        }

        private void OpenInventoryWindow(InputAction.CallbackContext _, bool fromClick)
        {
            Ray ray = Camera.main.ScreenPointToRay(playerControls.Player.MousePosition.ReadValue<Vector2>());
            RaycastHit2D hit = Physics2D.Raycast(ray.origin, ray.direction);
            if (hit && hit.collider != null)
            {
                Debug.Log("Opening inventory window");
                IFactoryElement block = hit.collider.GetComponent<IFactoryElement>();
                if (fromClick && SettingUI.Instance.Showing)
                {
                    return;
                }
                if (block != null)
                {
                    if (InventoryUI.Instance.Showing && InventoryUI.Instance.ShowingFactory)
                    {
                        InventoryUI.Instance.Hide();
                        SettingUI.Instance.Hide();
                    }
                    else
                    {
                        if (block is BufferBlock bufferBlock)
                            InventoryUI.Instance.Show(bufferBlock);
                        else if (hit.transform.tag == "Market")
                        {
                            InventoryUI.Instance.ShowMarket();
                        } else 
                            InventoryUI.Instance.Show();
                        SettingUI.Instance.Show(block);
                    }
                    return;
                }
                // Opening market
                if (hit.collider.CompareTag("Market"))
                {
                    InventoryUI.Instance.ShowMarket();
                    return;
                }
            }

            if (InventoryUI.Instance.Showing && !fromClick) 
            {
                InventoryUI.Instance.Hide();
                SettingUI.Instance.Hide();

            }
            else if (!fromClick)
            {
                InventoryUI.Instance.Show();
            }
            
        }

        private void OnEscapePressed(InputAction.CallbackContext ctx)
        {
            InventoryUI.Instance.Hide();
            SettingUI.Instance.Hide();
        }

        public bool AddResource(Resource resource)
        {
            if (inventory.TryGetValue(resource.ResourceType, out var slot)) 
                slot.AddResource(resource);
            else
                inventory.Add(resource.ResourceType, ResourceStack.Create(resource.ResourceType));
            return true;
        }
        public int GetResourceAmount(ResourceType resourceType)
        {
            if (!inventory.TryGetValue(resourceType, out var value)) return 0;
            return value.Quantity;
        }

        public ResourceStack RemoveStack(ResourceType resourceType)
        {
            ResourceStack stack = inventory[resourceType];
            inventory.Remove(resourceType);
            return stack;
        }

        public bool HasItem(ResourceType resourceType)
        {
            return inventory.Keys.ToArray().Contains(resourceType);
        }

        public void AddStack(ResourceStack resourceStack)
        {
            if (!inventory.TryAdd(resourceStack.ResourceType, resourceStack))
            {
                while (resourceStack.Quantity > 0)
                {
                    inventory[resourceStack.ResourceType].AddResource(resourceStack.TakeResource());
                }
            }
        }

        public Resource RemoveItem(ResourceType resourceType)
        {
            if (!inventory.ContainsKey(resourceType))
            {
                return null;
            }

            if (inventory[resourceType].Quantity > 0)
            {
                return inventory[resourceType].TakeResource();
            }
            else
            {
                return null;
            }
        }
    }
}