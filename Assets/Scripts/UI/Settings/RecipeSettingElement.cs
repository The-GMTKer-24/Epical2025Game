using Factory_Elements;
using Factory_Elements.Settings;
using TMPro;
using UnityEngine;

namespace UI.Inventory
{
    public class RecipeSettingElement : MonoBehaviour, ISettingElement
    {
        [SerializeField]
        private TextMeshProUGUI recipeText;
        [SerializeField]
        private TextMeshProUGUI settingNameText;
        [SerializeField]
        private TextMeshProUGUI settingDescriptionText;
        
        private ISetting recipeSetting;
        public void SetSetting(ISetting setting)
        {
            recipeSetting = setting;
            UpdateText();
        }

        public void UpdateText()
        {
            recipeText.text = ((Direction)recipeSetting.ValueUntyped).ToString();
            settingNameText.text = recipeSetting.Name;
            settingDescriptionText.text = recipeSetting.Description;
        }
        
        public void OnClickButton()
        {
            Direction activeDirection = (Direction)recipeSetting.ValueUntyped;
            activeDirection++;
            if (activeDirection > Direction.West)
            {
                activeDirection = Direction.North;
            }
            recipeSetting.ValueUntyped = activeDirection;
            UpdateText();
        }
    }
}