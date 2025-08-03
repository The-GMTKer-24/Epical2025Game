using System;
using System.Collections.Generic;
using Unity.Mathematics;

namespace Factory_Elements.Blocks
{
    public abstract class OneBlock : Block
    {
        protected Dictionary<Direction, IFactoryElement> directionalNeighbors;
        protected Dictionary<IFactoryElement, Direction> neighboralDirections; // lol
        
        public override void Awake()
        {
            base.Awake();
            
            directionalNeighbors = new Dictionary<Direction, IFactoryElement>();
            neighboralDirections = new Dictionary<IFactoryElement, Direction>();
        }
        public override void OnNeighborUpdate(IFactoryElement newNeighbor, bool added)
        {
            base.OnNeighborUpdate(newNeighbor, added);
            
            Dictionary<Direction, int2> relatives = new();
            relatives.Add(Direction.North, new int2(0, 1));
            relatives.Add(Direction.East, new int2(1, 0));
            relatives.Add(Direction.South, new int2(0, -1));
            relatives.Add(Direction.West, new int2(-1, 0));
            
            directionalNeighbors.Clear();
            neighboralDirections.Clear();

            foreach (Direction direction in Enum.GetValues(typeof(Direction)))
            {
                int2 checkPosition = position + relatives[direction];
                IFactoryElement neighbor = Factory.Instance.FromLocation(checkPosition);
                directionalNeighbors.Add(direction, neighbor);
                if (neighbor != null)
                    neighboralDirections.Add(neighbor, direction);
            }
        }
        
        public static Direction OppositeDirection(Direction direction)
        {
            switch (direction)
            {
                case Direction.North:
                    return Direction.South;
                case Direction.East:
                    return Direction.West;
                case Direction.South:
                    return Direction.North;
                case Direction.West:
                    return Direction.East;
                default:
                    throw new System.ArgumentException("Invalid direction");
            }
        }
    }
}