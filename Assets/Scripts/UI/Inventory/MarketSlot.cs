using System;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.UI;

namespace UI.Inventory
{
    public class MarketSlot : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
    {
        [SerializeField] Image image;
        [SerializeField] Image background;
        [SerializeField] Image border;
        [SerializeField] Color backgroundColor;
        [SerializeField] Color hoverColor;
        [SerializeField] TextMeshProUGUI priceText;
        [SerializeField] TextMeshProUGUI nameText;

        [SerializeField] private Color clickColor;
        [SerializeField] private Color borderColor;
        
        [SerializeField] private float showHoveredAfter;
        

        private float mouseEnterAt;
        private bool mouseOver;
        private bool selected;

        public int ClicksSinceSelected { get; private set; }

        public delegate void MarketSlotEvent(MarketSlot slot);
        public event MarketSlotEvent OnSelect;
        
        private PlayerControls controls;


        private void Awake()
        {
            selected = false;
            controls = new PlayerControls();
            controls.UI.Click.performed += OnPlayerClick;
            border.color = borderColor;

            background.color = backgroundColor;
        }

        public void OnEnable()
        {
            controls.Enable();
        }
        public void OnDisable()
        {
            controls.Disable();
        }

      
        
        public void SetSprite(Sprite sprite)
        {
            if (sprite != null)
                image.sprite = sprite;
            else
                image.gameObject.SetActive(false);
        }

        public void SetName(string name)
        {
            nameText.text = name;
        }
        
        public void SetPrice(int price)
        {
            priceText.text = $"${price}";
        }
        
        public void Update()
        {
            if (mouseOver)
            {
                if (mouseEnterAt + showHoveredAfter <= Time.time && !selected)
                {
                    background.color = hoverColor;
                }
            }
            else
            {
                background.color = backgroundColor;
            }

        }
        private void OnPlayerClick(InputAction.CallbackContext obj)
        {
            selected = mouseOver;
            
            if (selected)
            {
                if (OnSelect != null) OnSelect.Invoke(this);
                ClicksSinceSelected = 0;
            }
        }
        public void OnPointerEnter(PointerEventData eventData)
        {
            mouseEnterAt = Time.time;
            background.color = hoverColor;
            mouseOver = true;
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            background.color = backgroundColor;
            mouseOver = false;
        }
        
        
    }
}