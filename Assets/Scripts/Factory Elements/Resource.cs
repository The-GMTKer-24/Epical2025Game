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
    }
}