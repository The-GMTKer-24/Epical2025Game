﻿using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

namespace Scriptable_Objects
{
    [CreateAssetMenu(fileName = "New Factory Element", menuName = "Factory/Element", order = 0)]
    public class FactoryElementType : ScriptableObject
    {
        [SerializeField] private List<ResourceQuantity> cost;
        [SerializeField] private GameObject prefab;
        [SerializeField] private int2 size;


        public List<ResourceQuantity> Cost => cost;
        public GameObject Prefab => prefab;

        public int2 Size => size;
    }
}