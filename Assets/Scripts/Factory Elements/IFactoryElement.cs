using System.Drawing;
using Factory_Elements;
using Unity.Mathematics;

public interface IFactoryElement
{
    /// <summary>
    /// Returns the element's position in 2D space (each unit is one cell)
    /// If the element takes multiple cells of space, represents the bottom left corner (lowest x and y)
    /// </summary>
    int2 GetPosition();
    
    /// <summary>
    /// Returns the element's size (width in the x direction and depth in y)
    /// </summary>
    /// <returns></returns>
    int2 GetSize();
    
    /// <summary>
    /// Updates the element with new information about its neighbors
    /// </summary>
    /// <param name="newNeighbor">The neighbor that has been altered</param>
    /// <param name="added">True if the neighbor was added, false if removed</param>
    void OnNeighborUpdate(IFactoryElement newNeighbor, bool added);
    
    /// <summary>
    /// Returns whether this element will take an item or not
    /// </summary>
    bool AcceptsItem(IFactoryElement sender, InFlightItem item);
    
    /// <summary>
    /// Returns true if the item was successfully inserted
    /// </summary>
    bool TryInsertItem(IFactoryElement sender, InFlightItem item);
    
    /// <summary>
    /// This method should return the settings for this factory element
    /// </summary>
    /// <returns></returns>
    ISetting[] GetSettings();
}

