using System;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.UI;

namespace UI.Inventory
{
    public class InventorySlot : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
    {
        [SerializeField] Image image;
        [SerializeField] Image background;
        [SerializeField] Image border;
        [SerializeField] Color backgroundColor;
        [SerializeField] Color hoverColor;

        [SerializeField] private Color clickColor;
        [SerializeField] private Color selectedColor;
        [SerializeField] private Color hoverSelectedColor;
        [SerializeField] private Color borderColor;
        [SerializeField] private Color selectedBorderColor;
        
        [SerializeField] private float showHoveredAfter;
        [SerializeField] private float showTooltipAfter;
        
        
        
        
        [SerializeField] TextMeshProUGUI itemCount;
    
        private string tooltip;

        private float mouseEnterAt;
        private float lastMouseMoveAt;
        private bool mouseOver;
        private bool shownTooltip;
        private bool selected;

        public int ClicksSinceSelected { get; private set; }

        private PlayerControls controls;
        public delegate void InventorySlotEvent(InventorySlot slot);

        public event InventorySlotEvent OnUpdate;
        public event InventorySlotEvent OnSelect;
        public event InventorySlotEvent OnDeselect;

        private void Awake()
        {
            selected = false;
            shownTooltip = false;
            controls = new PlayerControls();
            controls.Player.MouseDelta.performed += OnMouseMove;
            controls.UI.Click.performed += OnPlayerClick;

            
            SetHoverText("Empty Slot");
            SetAmount("");
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
        
        public void SetHoverText(string text)
        {
            tooltip = text;
        }

        public void SetAmount(string amount)
        {
            itemCount.text = amount;
        }
        public void SetAmount(int amount)
        {
            itemCount.text = amount.ToString();
        }
        
        public void SetSprite(Sprite sprite)
        {
            if (sprite != null)
                image.sprite = sprite;
            else
                image.gameObject.SetActive(false);
        }

        public void Update()
        {
            if (OnUpdate != null) OnUpdate(this);
            if (mouseOver)
            {
                if (mouseEnterAt + showHoveredAfter <= Time.time && !selected)
                {
                    background.color = hoverColor;
                }
                else if (mouseEnterAt + showHoveredAfter <= Time.time && selected)
                {
                    background.color = hoverSelectedColor;
                }
                if (lastMouseMoveAt + showTooltipAfter <= Time.time && !shownTooltip)
                {
                    shownTooltip = true;
                    UITooltip.Instance.Show(tooltip);
                }
            }
            else if (selected)
            {
                background.color = selectedColor;
            }
            else
            {
                background.color = backgroundColor;
            }

            border.color = selected ? selectedBorderColor : borderColor;
        }
        private void OnPlayerClick(InputAction.CallbackContext obj)
        {
            selected = mouseOver;
            
            if (selected)
            {
                if (OnSelect != null) OnSelect.Invoke(this);
                ClicksSinceSelected = 0;
            }
            else
            {
                if (ClicksSinceSelected == 0)
                {
                    if (OnDeselect != null) OnDeselect.Invoke(this);
                }
                ClicksSinceSelected++;
            }
            
            if (shownTooltip)
            {
                UITooltip.Instance.Hide();
            }
        }
        private void OnMouseMove(InputAction.CallbackContext ctx)
        {
            lastMouseMoveAt = Time.time;
            if (shownTooltip)
            {
                UITooltip.Instance.Hide();
            }
        }
        public void OnPointerEnter(PointerEventData eventData)
        {
            mouseEnterAt = Time.time;
            mouseOver = true;
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            mouseOver = false;
        }
        
        
    }
}