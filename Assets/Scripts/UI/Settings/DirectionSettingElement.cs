using Factory_Elements;
using Factory_Elements.Settings;
using TMPro;
using UnityEngine;

namespace UI.Inventory
{
    public class DirectionSettingElement : MonoBehaviour, ISettingElement
    {
        [SerializeField]
        private TextMeshProUGUI directionText;
        [SerializeField]
        private TextMeshProUGUI settingNameText;
        [SerializeField]
        private TextMeshProUGUI settingDescriptionText;
        
        private ISetting directionSetting;
        public void SetSetting(ISetting setting)
        {
            directionSetting = setting;
            UpdateText();
        }

        public void UpdateText()
        {
            directionText.text = ((Direction)directionSetting.ValueUntyped).ToString();
            settingNameText.text = directionSetting.Name;
            settingDescriptionText.text = directionSetting.Description;
        }
        
        public void OnClickButton()
        {
            Direction activeDirection = (Direction)directionSetting.ValueUntyped;
            activeDirection++;
            if (activeDirection > Direction.West)
            {
                activeDirection = Direction.North;
            }
            directionSetting.ValueUntyped = activeDirection;
            UpdateText();
        }
    }
}