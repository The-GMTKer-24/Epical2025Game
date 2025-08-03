using System.Collections.Generic;
using System.Linq;
using Factory_Elements;
using Factory_Elements.Blocks;
using Factory_Elements.Settings;
using TMPro;
using UnityEngine;
using UnityEngine.Rendering;

namespace UI.Inventory
{
    public class DirectionalPipeSettings : MonoBehaviour, ISettingElement
    {

        [SerializeField]
        private TextMeshProUGUI settingNameText;
        [SerializeField]
        private TextMeshProUGUI settingDescriptionText;
        [SerializeField]
        private TMP_Dropdown pipeLocationDropdown;
        [SerializeField]
        private TMP_Dropdown pipeFluidDropdown;

        private ISetting flowSettings;
        private OutputPipeSetting flow;
        public void SetSetting(ISetting setting)
        {
            flowSettings = setting;
            flow = flowSettings.ValueUntyped as OutputPipeSetting;
            UpdateText();
        }
        public void UpdateText()
        {
            flow = flowSettings.ValueUntyped as OutputPipeSetting;
            if (flow == null)
            {
                settingNameText.text = "Pipe settings";
                settingDescriptionText.text = "No pipe settings exist for the current pipe settings";
                pipeLocationDropdown.gameObject.SetActive(false);
                pipeFluidDropdown.gameObject.SetActive(false);
                return;
            }
            pipeLocationDropdown.gameObject.SetActive(true);
            pipeFluidDropdown.gameObject.SetActive(true);
            settingNameText.text = flowSettings.Name;
            settingDescriptionText.text = flowSettings.Description;

            pipeLocationDropdown.ClearOptions();
            pipeFluidDropdown.ClearOptions();
            foreach (var location in flow.PipeSettingsFromLocation.ToList().OrderBy(x => x.Key))
            {
                pipeLocationDropdown.options.Add(
                    new TMP_Dropdown.OptionData($"{location.Key.Direction.ToString()}#{location.Key.Index.ToString()} - {(location.Value == null ? "None" : location.Value.name)}"));
            }
            pipeLocationDropdown.onValueChanged.RemoveAllListeners();
            pipeLocationDropdown.onValueChanged.AddListener(delegate
            {
                OutputLocation location =
                    flow.PipeSettingsFromLocation.ToList().OrderBy(x => x.Key).ToList()[pipeLocationDropdown.value].Key;
                pipeFluidDropdown.ClearOptions();
                for (var i = 0; i < flow.AllowedFluidTypes.Count; i++)
                {
                    var fluidType = flow.AllowedFluidTypes[i];
                    string text = (fluidType != null ? fluidType.name : "None");
                    pipeFluidDropdown.options.Add(new TMP_Dropdown.OptionData(text));
                    Debug.Log(text);
                    Debug.Log(fluidType);
                    if ((fluidType == null && flow.PipeSettingsFromLocation[location] == null) || (fluidType?.name == flow.PipeSettingsFromLocation[location]?.name))
                    {
                        pipeFluidDropdown.SetValueWithoutNotify(i);
                    }
                }
                pipeFluidDropdown.onValueChanged.RemoveAllListeners();
                pipeFluidDropdown.onValueChanged.AddListener((call) =>
                {
                    flow.PipeSettingsFromLocation[location] = flow.AllowedFluidTypes[pipeFluidDropdown.value];
                    flowSettings.ValueUntyped = flow;
                    flow = null;
                    UpdateText();
                });
                pipeFluidDropdown.value = 0;
                pipeFluidDropdown.RefreshShownValue();
            });
            pipeLocationDropdown.value = 1;
            pipeLocationDropdown.value = 0;
            pipeLocationDropdown.RefreshShownValue();
        }
        
    }
}