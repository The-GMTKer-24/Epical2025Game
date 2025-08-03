
using System.Linq;
using Factory_Elements;
using Factory_Elements.Blocks;
using Factory_Elements.Settings;
using Scriptable_Objects;
using TMPro;
using UnityEngine;

namespace UI.Inventory
{
    public class RecipeSetttingElement : MonoBehaviour, ISettingElement
    {
        [SerializeField]
        private TMP_Dropdown dropdown;
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
            dropdown.onValueChanged.RemoveAllListeners();
            dropdown.options.Clear();
            for (var i = 0; i < ((RecipeSetting)recipeSetting.ValueUntyped).Machine.Recipes.Length; i++)
            {
                var recipe = ((RecipeSetting)recipeSetting.ValueUntyped).Machine.Recipes[i];
                dropdown.options.Add(new TMP_Dropdown.OptionData(recipe.name));
                if (recipe.name == ((RecipeSetting)recipeSetting.ValueUntyped).Recipe.name)
                {
                    dropdown.SetValueWithoutNotify(i);
                }
            }

            dropdown.onValueChanged.AddListener(delegate
            {
                recipeSetting.ValueUntyped = new RecipeSetting(((RecipeSetting)recipeSetting.ValueUntyped).Machine.Recipes[dropdown.value],((RecipeSetting)recipeSetting.ValueUntyped).Machine);
                InventoryUI.Instance.Refresh();
                UpdateText();
            });
            ResourceQuantity[] inputs = ((RecipeSetting)recipeSetting.ValueUntyped).Recipe.Inputs;
            ResourceQuantity[] outputs = ((RecipeSetting)recipeSetting.ValueUntyped).Recipe.Outputs;
            float minimumTemperature = ((RecipeSetting)recipeSetting.ValueUntyped).Recipe.MinimumTemperature;
            string recipeIngridents = inputs.Aggregate("", (current, t) => current + $"{t.Amount} {t.Type.name} ");
            if (minimumTemperature > 0.0f)
            {
                recipeIngridents += $"@{minimumTemperature} ";
            }

            recipeIngridents += "=>";
            recipeIngridents = outputs.Aggregate(recipeIngridents, (current, t) => current + $"{t.Amount} {t.Type.name} ");
            recipeText.text = recipeIngridents;
            settingNameText.text = recipeSetting.Name;
            settingDescriptionText.text = recipeSetting.Description;
        }
    }
}