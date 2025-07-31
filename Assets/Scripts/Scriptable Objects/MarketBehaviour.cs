using UnityEngine;


[CreateAssetMenu(fileName = "Market Behaviour", menuName = "Items/Market Behaviour", order = 0)]
public class MarketBehaviour : ScriptableObject
{
    [SerializeField] private int standardPrice;
    [SerializeField] private int minPrice;
    [SerializeField] private int maxPrice;

    public int StandardPrice => standardPrice;

    public int MinPrice => minPrice;

    public int MaxPrice => maxPrice;
}