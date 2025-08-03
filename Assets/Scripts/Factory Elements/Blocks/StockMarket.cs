using System;
using System.Collections.Generic;
using System.Linq;
using Factory_Elements.Settings;
using Game_Info;
using Scriptable_Objects;
using UnityEngine;

namespace Factory_Elements.Blocks
{
    public class StockMarket : Block
    {
        [SerializeField] private ResourceSet sellableItems;
        public override Direction? Rotation { get; set; }
        public override bool Rotate(Direction direction)
        {
            throw new System.NotImplementedException();
        }

        public override bool SupportsRotation => false;
        public override bool AcceptsResource(IFactoryElement sender, Resource resource)
        {
            return true;
        }

        public override bool TryInsertResource(IFactoryElement sender, Resource resource)
        {
            if (Array.Exists(sellableItems.Resources, element => element==resource.ResourceType))
            {
                GameInfo.Instance.GainMoney((int)((ItemType)resource.ResourceType).MarketBehaviour.MaxPrice);
                return true;
            }
            
            return false;
            
        }

        public override ISetting[] GetSettings()
        {
            return Array.Empty<ISetting>();
        }

        public override Dictionary<ResourceType, int> GetHeldResources()
        {
            return new Dictionary<ResourceType, int>();
        }
    }
}