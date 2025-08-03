using System;

namespace Factory_Elements
{
    public enum Direction
    {
        North=0,
        East=1,
        South=2,
        West=3
    }

    public static class DirectionExtensions
    {
        public static Direction Invert(this Direction direction)
        {
            return direction switch
            {
                Direction.North => Direction.South,
                Direction.East => Direction.West,
                Direction.South => Direction.North,
                Direction.West => Direction.East,
                _ => throw new ArgumentOutOfRangeException(nameof(direction), direction, null)
            };
        }
    }
}