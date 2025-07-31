using Factory_Elements;

public interface IFactoryElement
{
    /// <summary>
    /// Updates a neighbor
    /// </summary>
    /// <param name="edge">The side that a new neighbor is created from</param>
    /// <param name="newNeighbor">New neighbor on the side. Null if the neighbor has been removed</param>
    void OnNeighborUpdate(Direction edge, IFactoryElement newNeighbor);
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

