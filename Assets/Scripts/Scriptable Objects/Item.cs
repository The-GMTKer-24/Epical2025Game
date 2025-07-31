using System;
using UnityEngine;

[CreateAssetMenu(fileName = "Item", menuName = "Items/Item")]
public class Item : ScriptableObject
{
    [SerializeField] private bool isFluid;
    [SerializeField] private float temperature;
    [SerializeField] private MarketBehaviour marketBehaviour;

    public bool IsFluid => isFluid;

    public float Temperature => temperature;

    public MarketBehaviour MarketBehaviour => marketBehaviour;
}
