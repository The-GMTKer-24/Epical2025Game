using System;
using System.Collections.Generic;
using Factory_Elements;
using UnityEngine;

namespace Player
{
    public class Player : MonoBehaviour
    {
        public static Player Instance { get; private set; }
        [SerializeField] private float speed;
        [SerializeField] private int maxInventorySize;
        private PlayerControls playerControls;
        private List<ItemStack> inventory = new();
        private void Awake()
        {
            Instance = this;
            playerControls = new PlayerControls();
        }

        // Update is called once per frame
        private void Update()
        {
            var scaledInput = playerControls.Player.Move.ReadValue<Vector2>() * (Time.deltaTime * speed);
            if (MathF.Abs(scaledInput.x) > 0)
            {
                transform.localScale = new Vector3(-MathF.Sign(scaledInput.x), 1, 1);
            }
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

        public int MaxInventorySize => maxInventorySize;

        public List<ItemStack> Inventory => inventory;
    }
}