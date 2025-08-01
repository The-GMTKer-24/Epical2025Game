using System;
using Factory_Elements.Settings;
using Scriptable_Objects;
using UnityEngine;

namespace Factory_Elements.Blocks
{
    public class Crafter : BufferBlock
    {
        protected ElementSettings<Recipe> recipeSetting;
        private float recipeProgress; // In seconds
        private bool running;

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