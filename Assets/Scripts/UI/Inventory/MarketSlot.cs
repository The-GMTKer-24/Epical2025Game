using System;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using Object = System.Object;

namespace UI.Inventory
{
    public class MarketSlot : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
    {
        [SerializeField] Image image;
        [SerializeField] Image background;
        [SerializeField] Image border;
        [SerializeField] Color backgroundColor;
        [SerializeField] Color hoverColor;
        [SerializeField] Color disabledColor;
        [SerializeField] TextMeshProUGUI priceText;
        [SerializeField] TextMeshProUGUI nameText;

        [SerializeField] private Color clickColor;
        [SerializeField] private Color borderColor;
        
        [SerializeField] private float showHoveredAfter;
        

        private float mouseEnterAt;
        private bool mouseOver;
        private bool selected;
        public bool disabled { get; private set; }

        public int ClicksSinceSelected { get; private set; }

        public delegate void MarketSlotEvent(MarketSlot slot);
        public event MarketSlotEvent OnSelect;
        
        private PlayerControls controls;


        private void Awake()
        {
            selected = false;
            disabled = false;
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

        public void SetDisabled(bool value)
        {
            disabled = value;
        }

        public void SetName(string name)
        {
            nameText.text = name;
        }
        
        public void SetPrice(float price)
        {
            priceText.text = $"${price}";
        }
        
        public void Update()
        {
            if (disabled)
            {
                backgroundColor = disabledColor;
                return;
            }
            
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
                return;
                ClicksSinceSelected = 0;
            }
        }
        public void OnPointerEnter(PointerEventData eventData)
        {
  
                mouseEnterAt = Time.time;
                if (!disabled)
                {
                    background.color = hoverColor;
                }

                mouseOver = true;
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            if (!disabled)
            {
                background.color = backgroundColor;
            }
            mouseOver = false;
        }
        
        
    }
}