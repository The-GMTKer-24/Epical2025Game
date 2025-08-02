using System;
using UnityEngine;

namespace Scriptable_Objects
{
    [Serializable]
    public struct MarketBehaviour 
    {
        [SerializeField] private float maxPrice;
        [SerializeField] private float priceDeltaPerUnit;
        [SerializeField] private float minPrice;
        
        [SerializeField] private float priceAddedFluctuationAverage;
        [SerializeField] private float priceFluctuationStandardDeviation;
        public float MinPrice => minPrice;

        public float MaxPrice => maxPrice;

        public float PriceDeltaPerUnit => priceDeltaPerUnit;
        
    }
}