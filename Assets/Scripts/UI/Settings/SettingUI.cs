using System;
using Factory_Elements;
using Factory_Elements.Settings;
using UnityEngine;

namespace UI.Inventory
{
    public class SettingUI : MonoBehaviour
    {
        public static SettingUI Instance;
        [SerializeField] private GameObject settingsPanel;
        [SerializeField] private GameObject contentPanel;

        [SerializeField] private GameObject directionSettingPrefab;
        public bool Showing { get; private set; }

        public void Awake()
        {
            Instance = this;
        }

        public void Start()
        {
            Hide();
        }

        public void Show(IFactoryElement element)
        {
            Hide();
            settingsPanel.SetActive(true);
            foreach (ISetting setting in element.GetSettings())
            {
                Instantiate(GetSetting(setting), contentPanel.transform).GetComponent<ISettingElement>().SetSetting(setting);
            }
            Showing = true;
        }

        public void Hide()
        {
            settingsPanel.SetActive(false);
            foreach (Transform child in contentPanel.transform)
            {
                Destroy(child.gameObject);
            }
            Showing = false;
        }

        private GameObject GetSetting(ISetting setting)
        {
            switch (setting.SettingType)
            {
                case SettingType.Direction:
                    return directionSettingPrefab;

                case SettingType.Bool:
                    break;
                case SettingType.DirectionalFlow:
                    break;
                case SettingType.Recipe:
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            return null;
        }
    }
}