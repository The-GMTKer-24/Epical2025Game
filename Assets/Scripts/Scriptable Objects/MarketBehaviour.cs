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
        [SerializeField] private float supplyDecayRate; // measured in inverse minutes, 0-1 - i.e. 0 means supply is instantly consumed and does not impact the market, and 1 means supply lasts forever and prices permanently drop
        public float MinPrice => minPrice;

        public float MaxPrice => maxPrice;

        public float PriceDeltaPerUnit => priceDeltaPerUnit;
        
    }
}