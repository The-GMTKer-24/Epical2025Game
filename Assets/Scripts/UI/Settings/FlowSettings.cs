using System.Collections.Generic;
using System.Linq;
using Factory_Elements;
using Factory_Elements.Blocks;
using Factory_Elements.Settings;
using Scriptable_Objects;
using TMPro;
using UnityEngine;
using UnityEngine.Rendering;

namespace UI.Inventory
{
    public class FlowSettings : MonoBehaviour, ISettingElement
    {

        [SerializeField]
        private TextMeshProUGUI settingNameText;
        [SerializeField]
        private TextMeshProUGUI settingDescriptionText;
        [SerializeField]
        private RectTransform toggleInput;
        [SerializeField]
        private TMP_Dropdown sortMode;

        [SerializeField] private TextMeshProUGUI inputEnabled;

        [SerializeField] private ResourceSet allItems;
        private ISetting flowSettings;
        private DirectionConfig flow;
        public void SetSetting(ISetting setting)
        {
            flowSettings = setting;
            flow = flowSettings.ValueUntyped as DirectionConfig;
            UpdateText();
        }

        public void UpdateText()
        {
            flow = flowSettings.ValueUntyped as DirectionConfig;
            if (flow == null)
            {
                settingNameText.text = "Flow Settings";
                settingDescriptionText.text = "No flow options are available";
                return;
            }

            sortMode.gameObject.SetActive(true);
            settingNameText.text = flowSettings.Name;
            settingDescriptionText.text = flowSettings.Description;
            if (flow.Input)
            {
                sortMode.gameObject.SetActive(false);
            }
            else
            {
                sortMode.gameObject.SetActive(true);
                sortMode.ClearOptions();
                for (var index = 0; index < allItems.Resources.Length; index++)
                {
                    var item = allItems.Resources[index];
                    sortMode.options.Add(new TMP_Dropdown.OptionData(item.name));
                    if ((item == null && flow.SortType == null) || (item?.name == flow.SortType?.name))
                    {
                        sortMode.SetValueWithoutNotify(index);
                    }
                }
                sortMode.RefreshShownValue();
                sortMode.onValueChanged.RemoveAllListeners();
                sortMode.onValueChanged.AddListener(delegate
                {
                    flow.SortType = allItems.Resources[sortMode.value];
                    flowSettings.ValueUntyped = flow;
                    UpdateText();
                });
            }
        }

        public void OnButtonPress()
        {
            flow.Input = !flow.Input;
            inputEnabled.text = flow.Input ? "Input" : "Output";
            flowSettings.ValueUntyped = flow;
            UpdateText();
        }
    }
}