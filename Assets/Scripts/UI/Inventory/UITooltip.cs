using System;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;

namespace UI.Inventory
{
    public class UITooltip : MonoBehaviour
    {
        public static UITooltip Instance;
        [SerializeField] private TextMeshProUGUI tooltipText;
        private PlayerControls controls;
        public void Awake()
        {
            Instance = this;
            controls = new PlayerControls();
            Hide();
        }

        private void OnEnable()
        {
            controls.Enable();
        }

        private void OnDisable()
        {
            controls.Disable();
        }
        public void Update()
        {
            transform.position = controls.Player.MousePosition.ReadValue<Vector2>();
        }

        public void Show(string tooltip)
        {
            if (string.IsNullOrWhiteSpace(tooltip))
                return;
            this.tooltipText.text = tooltip;
            this.gameObject.SetActive(true);
        }

        public void Hide()
        {
            
            this.gameObject.SetActive(false);
        }
    }
}