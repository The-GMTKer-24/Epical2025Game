using Factory_Elements.Settings;
using Scriptable_Objects;
using Unity.Mathematics;
using UnityEngine;

namespace Factory_Elements.Blocks
{
    public class ItemSource : MonoBehaviour, IFactoryElement
    {
        [SerializeField] private FactoryElementType type;
        [SerializeField] private ResourceSet pool;
        
        // Start is called once before the first execution of Update after the MonoBehaviour is created
        void Start()
        {
        
        }

        // Update is called once per frame
        void Update()
        {
        
        }

        public int2 Position { get; set; }
        public FactoryElementType FactoryElementType { get => type; }
    
        public void OnNeighborUpdate(IFactoryElement newNeighbor, bool added)
        {
            Debug.Log($"{newNeighbor.FactoryElementType.name} has been changed");
        }

        public bool AcceptsResource(IFactoryElement sender, Resource resource)
        {
            return false;
        }

        public bool TryInsertResource(IFactoryElement sender, Resource resource)
        {
            return false;
        }

        public ISetting[] GetSettings()
        {
            throw new System.NotImplementedException();
        }
    }
}
