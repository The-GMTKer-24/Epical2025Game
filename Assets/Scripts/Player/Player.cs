using System;
using System.Collections.Generic;
using System.Linq;
using Factory_Elements;
using Factory_Elements.Blocks;
using Scriptable_Objects;
using UI.Inventory;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Player
{
    public class Player : MonoBehaviour
    {
        [SerializeField] private float speed;
        [SerializeField] private int maxInventorySize;
        private readonly Dictionary<ResourceType, ResourceStack> inventory = new();
        private PlayerControls playerControls;
        public static Player Instance { get; private set; }


        public int MaxInventorySize => maxInventorySize;

        public List<ResourceStack> Inventory => inventory.Values.ToList();

        private void Awake()
        {
            Instance = this;
            playerControls = new PlayerControls();
            playerControls.Player.Interact.performed += OpenInventoryWindow;
            playerControls.Player.Cancel.performed += OnEscapePressed;
        }

        // Update is called once per frame
        private void Update()
        {
            var scaledInput = playerControls.Player.Move.ReadValue<Vector2>() * (Time.deltaTime * speed);
            if (MathF.Abs(scaledInput.x) > 0) transform.localScale = new Vector3(-MathF.Sign(scaledInput.x), 1, 1);
            transform.transform.position += new Vector3(scaledInput.x, scaledInput.y, 0);
        }

        private void OnEnable()
        {
            playerControls.Enable();
        }

        private void OnDisable()
        {
            playerControls.Disable();
        }

        private void OpenInventoryWindow(InputAction.CallbackContext ctx)
        {
            Ray ray = Camera.main.ScreenPointToRay(playerControls.Player.MousePosition.ReadValue<Vector2>());
            RaycastHit2D hit = Physics2D.Raycast(ray.origin, ray.direction);
            
            if (hit && hit.collider != null)
            {
                BufferBlock block = hit.collider.GetComponent<BufferBlock>();
                if (block)
                {
                    InventoryUI.Instance.Show(hit.collider.GetComponent<BufferBlock>());
                    return;
                }
            }
            InventoryUI.Instance.Show();
        }

        private void OnEscapePressed(InputAction.CallbackContext ctx)
        {
            InventoryUI.Instance.Hide();
        }
        

        public bool AddResource(Resource resource)
        {
            if (inventory.Count < maxInventorySize)
                inventory.Add(resource.ResourceType, ResourceStack.Create(resource.ResourceType));
            else
                return false;
            inventory[resource.ResourceType].AddResource(resource);
            return true;
        }
        
        // TODO: Allow for the either insertion or complete failure of a quantity of resources

        public int GetResourceAmount(ResourceType resourceType)
        {
            // TODO: implement for fluids and whateverrr
            if (!inventory.ContainsKey(resourceType)) return 0;
            return inventory[resourceType].Quantity;
        }

        public void ConsumeResource(ResourceQuantity resourceQuantity)
        {
            // TODO: implement for fluids and whatever if inventory can take fluids maybe it already can idk
            // TODO: also needs overhauling if inventory system changes
            if (!inventory.ContainsKey(resourceQuantity.Type)) throw new Exception("Not enough resources to consume");
            if (inventory[resourceQuantity.Type].Quantity < resourceQuantity.Amount) throw new Exception("Not enough resources to consume");
            for (int i = 0; i < resourceQuantity.Amount; i++)
            {
                inventory[resourceQuantity.Type].TakeResource();
            }
        }
    }
}