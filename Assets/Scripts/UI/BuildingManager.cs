using System;
using System.Collections.Generic;
using Factory_Elements;
using Scriptable_Objects;
using Unity.Mathematics;

namespace Game_Info
{
    /// <summary>
    /// Manages the construction of buildings
    /// </summary>
    public static class BuildingManager
    {
        // Replaces all unowned machine parts with money
        // First tuple value is the parts that would be consumed - ALL of these parts are by necessity already held in the inventory
        // Second value is the money that would need to be spent to cover the remaining parts - There is no guarantee this money is available
        private static Tuple<List<ResourceQuantity>, int> EvaluateCost(IEnumerable<ResourceQuantity> items)
        {
            List<ResourceQuantity> itemCost = new List<ResourceQuantity>();
            int moneyCost = 0;
            foreach (ResourceQuantity itemQuantity in items)
            {
                ResourceType type = itemQuantity.Type;
                int costAmount = itemQuantity.Amount;
                int available = Player.Player.Instance.GetResourceAmount(type);
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
        
        /// <summary>
        /// Finds if a building can be placed in a given location, based on whether that location is clear, as well as whether the player can afford it.
        /// </summary>
        public static bool CanBuild(FactoryElementType building, int2 location)
        {
            if (!Factory.Instance.CanPlace(building, location)) return false;
            int cost = EvaluateCost(building.Cost).Item2;
            if (cost > GameInfo.Instance.Money) return false;
            return true;
        }

        /// <summary>
        /// Attempts to build a building, consuming the requisite resources along the way.
        /// </summary>
        /// <returns>Whether the construction was successful</returns>
        public static bool TryBuild(FactoryElementType building, int2 location, Direction rotation)
        {
            if (!CanBuild(building, location)) return false;
            Tuple<List<ResourceQuantity>, int> fullCost = EvaluateCost(building.Cost);
            // Consume resources
            GameInfo.Instance.SpendMoney(fullCost.Item2);
            foreach (ResourceQuantity itemCost in fullCost.Item1)
            {
                Player.Player.Instance.ConsumeResource(itemCost);
            }
            // Build the building
            Factory.Instance.TryPlace(building, location, rotation,out _);
            return true;
        }

        /// <summary>
        /// Finds if it is allowable to deconstruct a building - Ensuring all contained items can fit into the inventory
        /// </summary>
        /// <param name="location"></param>
        /// <returns></returns>
        public static bool CanDeconstruct(int2 location)
        {
            // TODO: I don't think inventory is in a state that this can be implemented yet
            throw new NotImplementedException();
        }
    }
}