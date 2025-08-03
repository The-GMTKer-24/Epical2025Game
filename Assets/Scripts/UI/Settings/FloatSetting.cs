using System.Globalization;
using Factory_Elements.Settings;
using TMPro;
using UnityEngine;

namespace UI.Inventory
{
    public class FloatSetting : MonoBehaviour, ISettingElement
    {
        [SerializeField]
        private TMP_InputField inputField;
        [SerializeField]
        private TextMeshProUGUI settingNameText;
        [SerializeField]
        private TextMeshProUGUI settingDescriptionText;
        
        private ISetting floatSetting;
        public void SetSetting(ISetting setting)
        {
            floatSetting = setting;
            UpdateText();
        }

        public void UpdateText()
        {
            settingNameText.text = floatSetting.Name;
            settingDescriptionText.text = floatSetting.Description;
            inputField.text = ((float)floatSetting.ValueUntyped).ToString(CultureInfo.InvariantCulture);
        }

        public void OnButtonClick()
        {
            if (float.TryParse(inputField.text, out float value))
                floatSetting.ValueUntyped = value;
            UpdateText();
        }
    }
}