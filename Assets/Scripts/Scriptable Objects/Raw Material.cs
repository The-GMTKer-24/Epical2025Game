using System;
using UnityEngine;

[CreateAssetMenu(fileName = "RawMaterial", menuName = "Scriptable Objects/RawMaterial")]
public class RawMaterial : ScriptableObject
{
    [SerializeField] private new string name;
    
    
    public string Name => name;
}
