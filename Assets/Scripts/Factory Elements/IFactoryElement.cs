using System.Collections.Generic;
using Factory_Elements.Settings;
using Scriptable_Objects;
using Unity.Mathematics;

namespace Factory_Elements
{
    public interface IFactoryElement
    {
        /// <summary>
        ///     The element's position in 2D space (each unit is one cell)
        ///     If the element takes multiple cells of space, represents the bottom left corner (lowest x and y)
        /// </summary>
        public int2 Position { get; set; }

        /// <summary>
        /// The current rotation of the machine. Null if the machine does not support rotation
        /// </summary>
        public Direction? Rotation { get; }

        /// <summary>
        /// Changes the rotation of the machine. Throws a not implemented exception if the machine does not support rotation
        /// </summary>
        /// <param name="direction">The direction that you want the element to face in</param>
        /// <returns>True if the rotation was successful. False if rotation was unable to be performed</returns>
        bool Rotate(Direction direction);
        
        /// <summary>
        /// Returns if the machine supports rotation.
        /// </summary>
        public bool SupportsRotation { get; }

        /// <summary>
        ///     Reference to the corresponding scriptable object
        /// </summary>
        public FactoryElementType FactoryElementType { get; }

        /// <summary>
        ///     Updates the element with new information about its neighbors
        /// </summary>
        /// <param name="newNeighbor">The neighbor that has been altered</param>
        /// <param name="added">True if the neighbor was added, false if removed</param>
        void OnNeighborUpdate(IFactoryElement newNeighbor, bool added);

        /// <summary>
        ///     Returns whether this element will take an item or not
        /// </summary>
        bool AcceptsResource(IFactoryElement sender, Resource resource);

        /// <summary>
        ///     Returns true if the item was successfully inserted
        /// </summary>
        bool TryInsertResource(IFactoryElement sender, Resource resource);

        /// <summary>
        ///     This method should return the settings for this factory element
        /// </summary>
        /// <returns></returns>
        ISetting[] GetSettings();

        /// <summary>
        /// The extra items and liquids (not counting the parts the block comprises) that should be returned to the inventory upon deconstruction
        /// </summary>
        /// <returns>A mapping of all the included resource types, to the amount of that type that is included</returns>
        Dictionary<ResourceType, int> GetHeldResources();
    }
}