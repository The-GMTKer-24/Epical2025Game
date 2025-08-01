using System.Collections.Generic;
using Factory_Elements.Settings;
using Scriptable_Objects;
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
            recipeSetting = new ElementSettings<Recipe>(defaultRecipe, "Active Recipe",
                "The recipe that this machine is currently using");
            RecipeUpdate();
            recipeSetting.SettingUpdated += RecipeUpdate;
            recipeProgress = 0;
            running = false;
        }

        public void Start()
        {
            RecipeUpdate();
        }

        public void FixedUpdate()
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
                    recipeProgress = 0;
                    // Recipe finished!
                    Debug.Log("Crafted!");
                    foreach (var resourceQuantity in recipeSetting.Value.Inputs)
                    {
                        var resourceType = resourceQuantity.Type;
                        buffers[resourceType].ConsumeResources(resourceQuantity.Amount);
                        Debug.Log("Consumed:");
                        Debug.Log(resourceQuantity.Type.name);
                        Debug.Log(resourceQuantity.Amount.ToString());
                    }

                    foreach (var resourceQuantity in recipeSetting.Value.Outputs)
                    {
                        var resourceType = resourceQuantity.Type;
                        buffers[resourceType].CreateResources(resourceQuantity.Amount);
                        Debug.Log("Created:");
                        Debug.Log(resourceQuantity.Type.name);
                        Debug.Log(resourceQuantity.Amount.ToString());
                    }
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

        public override ISetting[] GetSettings()
        {
            return new[] { recipeSetting };
        }
    }
}