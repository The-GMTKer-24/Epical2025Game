using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

namespace UI.Inventory
{
    public class InventorySlot : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
    {
        [SerializeField]
        SpriteRenderer spriteRenderer;
        [SerializeField]
        TextMeshProUGUI itemCount;
        [SerializeField]
        TextMeshProUGUI tooltip;
        public void SetHoverText(string text)
        {
            tooltip.text = text;
        }

        public void SetAmount(int amount)
        {
            itemCount.text = amount.ToString();
        }

        public void SetSprite(Sprite sprite)
        {
            spriteRenderer.sprite = sprite;
        }

        public void Update()
        {
            if (!string.IsNullOrEmpty(tooltip.text) && tooltip.text.Length > 0 && tooltip.gameObject.activeInHierarchy)
                tooltip.transform.position = Input.mousePosition;
        }
        public void OnPointerEnter(PointerEventData eventData)
        {
            tooltip.gameObject.SetActive(true);
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            tooltip.gameObject.SetActive(false);
        }
    }
}