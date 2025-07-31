using System;
using UnityEngine;

[CreateAssetMenu(fileName = "Item", menuName = "Items/Item")]
public class ItemType : ScriptableObject
{
    [SerializeField] private MarketBehaviour marketBehaviour;

    public MarketBehaviour MarketBehaviour => marketBehaviour;
}
