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
        [SerializeField] private float thermalLoss = 0; // 0-1, 0 is perfect, 1 is back to room temp
        [SerializeField] private Recipe defaultRecipe;
        private float recipeProgress; // In seconds
        protected ElementSettings<RecipeSetting> RecipeSetting;
        private bool running;

        public void Awake()
        {
            base.Awake();
            RecipeSetting = new ElementSettings<RecipeSetting>(new RecipeSetting(defaultRecipe, factoryElementType as Machine), "Active Recipe",
                "The recipe that this machine is currently using", SettingType.Recipe);
            RecipeUpdate();
            RecipeSetting.SettingUpdated += RecipeUpdate;
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
            if (resource is Item item && item.Temperature < RecipeSetting.Value.Recipe.MinimumTemperature) return false;
            return base.AcceptsResource(sender, resource);
        }

        public virtual void Craft()
        {
            recipeProgress = 0;
            Debug.Log("Crafted!");
            float sumTemperature = 0;
            float samples = 0;
            foreach (ResourceQuantity resourceQuantity in RecipeSetting.Value.Recipe.Inputs)
            {
                ResourceType resourceType = resourceQuantity.Type;
                for (int i = 0; i < resourceQuantity.Amount; i++)
                {
                    Resource resource = buffers[resourceType].TakeResource();
                    if (resource is Item item)
                    {
                        sumTemperature += item.Temperature;
                        samples++;
                    }
                }
            }

            float averageTemperature = Factory.Instance.roomTemperature;
            if (samples != 0)
            {
                averageTemperature = sumTemperature / samples;
            }
            float displacement = averageTemperature - Factory.Instance.roomTemperature;
            float newTemperature = displacement * (1 - thermalLoss) + Factory.Instance.roomTemperature;

            
            foreach (ResourceQuantity resourceQuantity in RecipeSetting.Value.Recipe.Outputs)
            {
                ResourceType resourceType = resourceQuantity.Type;
                int resourceAmount = resourceQuantity.Amount;
                for (int i = 0; i < resourceQuantity.Amount; i++)
                {
                    Resource resource = Resource.fromType(resourceType);
                    if (resource is Item item)
                    {
                        item.Temperature = newTemperature;
                    }
                    buffers[resourceType].AddResource(resource);
                }
            }
        }

        protected override void FixedUpdate()
        {
            base.FixedUpdate();
            
            var canRun = true;
            foreach (var resourceQuantity in RecipeSetting.Value.Recipe.Inputs)
            {
                var resourceType = resourceQuantity.Type;
                if (buffers[resourceType].Quantity < resourceQuantity.Amount) canRun = false;
            }

            foreach (var resourceQuantity in RecipeSetting.Value.Recipe.Outputs)
            {
                var resourceType = resourceQuantity.Type;
                var remainingSpace = buffers[resourceType].Capacity - buffers[resourceType].Quantity;
                if (remainingSpace < resourceQuantity.Amount) canRun = false;
            }

            running = canRun;

            if (running)
            {
                recipeProgress += Time.fixedDeltaTime;
                var recipe = RecipeSetting.Value;
                if (recipeProgress >= recipe.Recipe.ProcessingTime)
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
            foreach (var resourceQuantity in RecipeSetting.Value.Recipe.Inputs)
                newBuffers.Add(new Buffer(resourceQuantity.Amount * 5, resourceQuantity.Type, true, false));
            foreach (var resourceQuantity in RecipeSetting.Value.Recipe.Outputs)
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
            return settings.Append(RecipeSetting).ToArray();
        }
    }

    public struct RecipeSetting
    {
        public RecipeSetting(Recipe recipe, Machine machine)
        {
            this.Recipe = recipe;
            this.Machine = machine;
        }

        public Recipe Recipe { get; }

        public Machine Machine { get; }
    }
}