using System;
using System.Collections.Generic;
using Game_Info;
using Scriptable_Objects;
using Unity.Mathematics;

namespace Factory_Elements
{
    /// <summary>
    /// Manages the construction of buildings
    /// </summary>
    public static class BuildingManager
    {
        // Replaces all unowned machine parts with money
        // First tuple value is the parts that would be consumed - ALL of these parts are by necessity already held in the inventory
        // Second value is the money that would need to be spent to cover the remaining parts - There is no guarantee this money is available
        public static Tuple<List<ResourceQuantity>, int> EvaluateCost(IEnumerable<ResourceQuantity> items, bool ignoreInventory=false)
        {
            List<ResourceQuantity> itemCost = new List<ResourceQuantity>();
            int moneyCost = 0;
            foreach (ResourceQuantity itemQuantity in items)
            {
                ResourceType type = itemQuantity.Type;
                int costAmount = itemQuantity.Amount;
                int available = 0;
                if (!ignoreInventory)
                {
                     available = Player.Player.Instance.GetResourceAmount(type);
                }

                if (available >= costAmount)
                {
                    itemCost.Add(itemQuantity);
                }
                else
                {
                    itemCost.Add(new ResourceQuantity(type, available));
                    int remainder = costAmount - available;
                    moneyCost += remainder * ((ItemType)type).Cost;
                }
            }
            return new Tuple<List<ResourceQuantity>, int>(itemCost, moneyCost);
        }
    }
}