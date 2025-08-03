using System.Collections.Generic;
using System.Linq;
using Factory_Elements.Settings;
using Scriptable_Objects;
using UI.Inventory;
using UnityEngine;

namespace Factory_Elements.Blocks
{
    public class Crafter : BufferBlock
    {
        
        [SerializeField] private Recipe defaultRecipe;
        private float recipeProgress; // In seconds
        protected ElementSettings<Recipe> recipeSetting;
        private bool running;

        public void Awake()
        {
            base.Awake();
            recipeSetting = new ElementSettings<Recipe>(defaultRecipe, "Active Recipe",
                "The recipe that this machine is currently using", SettingType.Recipe);
            RecipeUpdate();
            recipeSetting.SettingUpdated += RecipeUpdate;
            recipeProgress = 0;
            running = false;
            equalizationRate = 0.0f; // Allowing items to cool and clog the machine would SUCK
        }

        public void Start()
        {
            RecipeUpdate();
        }

        public override bool AcceptsResource(IFactoryElement sender, Resource resource)
        {
            // Ensure the crafter only accepts items with high enough temperature
            if (resource is Item item && item.Temperature < recipeSetting.Value.MinimumTemperature) return false;
            return base.AcceptsResource(sender, resource);
        }

        public void Craft()
        {
            recipeProgress = 0;
            Debug.Log("Crafted!");
            foreach (var resourceQuantity in recipeSetting.Value.Inputs)
            {
                var resourceType = resourceQuantity.Type;
                buffers[resourceType].ConsumeResources(resourceQuantity.Amount);
            }

            foreach (var resourceQuantity in recipeSetting.Value.Outputs)
            {
                var resourceType = resourceQuantity.Type;
                buffers[resourceType].CreateResources(resourceQuantity.Amount);
            }
        }

        protected override void FixedUpdate()
        {
            base.FixedUpdate();
            
            var canRun = true;
            foreach (var resourceQuantity in recipeSetting.Value.Inputs)
            {
                var resourceType = resourceQuantity.Type;
                if (buffers[resourceType].Quantity < resourceQuantity.Amount) canRun = false;
            }

            foreach (var resourceQuantity in recipeSetting.Value.Outputs)
            {
                var resourceType = resourceQuantity.Type;
                var remainingSpace = buffers[resourceType].Capacity - buffers[resourceType].Quantity;
                if (remainingSpace < resourceQuantity.Amount) canRun = false;
            }

            running = canRun;

            if (running)
            {
                recipeProgress += Time.fixedDeltaTime;
                var recipe = recipeSetting.Value;
                if (recipeProgress >= recipe.ProcessingTime)
                {
                    Craft();
                }
            }
            else
            {
                recipeProgress = 0;
            }
        }

        private void RecipeUpdate()
        {
            var newBuffers = new List<Buffer>();
            foreach (var resourceQuantity in recipeSetting.Value.Inputs)
                newBuffers.Add(new Buffer(resourceQuantity.Amount * 5, resourceQuantity.Type, true, false));
            foreach (var resourceQuantity in recipeSetting.Value.Outputs)
                newBuffers.Add(new Buffer(resourceQuantity.Amount * 5, resourceQuantity.Type, false, true));
            setBuffers(newBuffers);
        }

        public override Direction? Rotation
        {
            get => null;
            set => throw new System.NotImplementedException();
        }

        public override bool Rotate(Direction direction)
        {
            throw new System.NotImplementedException();
        }

        public override bool SupportsRotation => false;

        public override ISetting[] GetSettings()
        {
            ISetting[] settings = base.GetSettings();
            return settings.Append(recipeSetting).ToArray();
        }
    }
}