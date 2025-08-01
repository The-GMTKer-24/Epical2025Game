using System;
using Scriptable_Objects;

namespace Factory_Elements
{
    [Serializable]
    public class Resource
    {
        public readonly ResourceType ResourceType;

        protected Resource(ResourceType resourceType)
        {
            ResourceType = resourceType;
        }

        public static Resource fromType(ResourceType resourceType)
        {
            if (resourceType is ItemType itemType) return new Item(itemType, Factory.Instance.roomTemperature);

            if (resourceType is FluidType fluidType) return new Fluid(fluidType);

            throw new Exception($"Resource type {resourceType} is not supported");
        }
    }
}