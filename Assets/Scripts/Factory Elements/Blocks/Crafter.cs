using System;
using System.Collections.Generic;
using Factory_Elements.Settings;
using Scriptable_Objects;
using UnityEngine;

namespace Factory_Elements.Blocks
{
    public class Crafter : BufferBlock
    {
        [SerializeField] Recipe defaultRecipe;
        protected ElementSettings<Recipe> recipeSetting;
        private float recipeProgress; // In seconds
        private bool running;

        public void Awake()
        {
            recipeSetting = new ElementSettings<Recipe>(defaultRecipe, "Active Recipe", "The recipe that this machine is currently using");
            RecipeUpdate();
            recipeSetting.SettingUpdated += RecipeUpdate;
            recipeProgress = 0;
            running = false;
        }

        public void Start()
        {
            RecipeUpdate();
        }

        private void RecipeUpdate()
        {
            List<Buffer> newBuffers = new List<Buffer>();
            foreach (ResourceQuantity resourceQuantity in recipeSetting.Value.Inputs)
            {
                newBuffers.Add(new Buffer(resourceQuantity.Amount * 5, resourceQuantity.Type, true, false));
            }
            foreach (ResourceQuantity resourceQuantity in recipeSetting.Value.Outputs)
            {
                newBuffers.Add(new Buffer(resourceQuantity.Amount * 5, resourceQuantity.Type, false, true));
            }
            setBuffers(newBuffers);
        }

        public void FixedUpdate()
        {
            base.FixedUpdate();
            
            bool canRun = true;
            foreach (ResourceQuantity resourceQuantity in recipeSetting.Value.Inputs)
            {
                ResourceType resourceType = resourceQuantity.Type;
                if (buffers[resourceType].Quantity < resourceQuantity.Amount)
                {
                    canRun = false;
                }
            }
            foreach (ResourceQuantity resourceQuantity in recipeSetting.Value.Outputs)
            {
                ResourceType resourceType = resourceQuantity.Type;
                int remainingSpace = buffers[resourceType].Capacity - buffers[resourceType].Quantity;
                if (remainingSpace < resourceQuantity.Amount)
                {
                    canRun = false;
                }
            }
            running = canRun;

            if (running)
            {
                recipeProgress += Time.fixedDeltaTime;
                Recipe recipe = recipeSetting.Value;
                if (recipeProgress >= recipe.ProcessingTime)
                {
                    recipeProgress = 0;
                    // Recipe finished!
                    foreach (ResourceQuantity resourceQuantity in recipeSetting.Value.Inputs)
                    {
                        ResourceType resourceType = resourceQuantity.Type;
                        buffers[resourceType].ConsumeResources(resourceQuantity.Amount);
                    }
                    foreach (ResourceQuantity resourceQuantity in recipeSetting.Value.Outputs)
                    {
                        ResourceType resourceType = resourceQuantity.Type;
                        buffers[resourceType].CreateResources(resourceQuantity.Amount);
                    }
                }
            }
            else
            {
                recipeProgress = 0;
            }
        }

        public override ISetting[] GetSettings()
        {
            return new [] { recipeSetting };
        }
    }
}